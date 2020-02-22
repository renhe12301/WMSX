using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using Web.WebServices.Interfaces;
using Web.WebServices.Models;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using ApplicationCore.Entities.FlowRecord;
using Ardalis.GuardClauses;
using NeoSmart.AsyncLock;
using IOrderService = ApplicationCore.Interfaces.IOrderService;

namespace Web.WebServices.Services
{
    public class OrderSOAPService:IOrderSOAPService
    {
        private readonly IOrderService _orderService;
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<OU> _ouRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<Supplier> _supplierRepository;
        private readonly IAsyncRepository<SupplierSite> _supplierSiteRepository;
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;
        private readonly IAsyncRepository<EBSProject> _ebsProjectRepository;
        private readonly IAsyncRepository<EBSTask> _ebsTaskRepository;
        private readonly IAsyncRepository<Employee> _employeeRepository;
        private readonly IAsyncRepository<Organization> _organizationRepository;
        private readonly IAsyncRepository<ReservoirArea> _areaRepository;
        private readonly ILogRecordService _logRecordService;

        public OrderSOAPService(IOrderService orderService,
                                IAsyncRepository<OU> ouRepository,
                                IAsyncRepository<Order> orderRepository,
                                IAsyncRepository<Warehouse> warehouseRepository,
                                IAsyncRepository<Supplier> supplierRepository,
                                IAsyncRepository<SupplierSite> supplierSiteRepository,
                                IAsyncRepository<MaterialDic> materialDicRepository,
                                IAsyncRepository<EBSProject> ebsProjectRepository,
                                IAsyncRepository<EBSTask> ebsTaskRepository,
                                IAsyncRepository<Employee> employeeRepository,
                                IAsyncRepository<Organization> organizationRepository,
                                IAsyncRepository<ReservoirArea> areaRepository,
                                ILogRecordService logRecordService)
        {
            this._orderService = orderService;
            this._ouRepository = ouRepository;
            this._orderRepository = orderRepository;
            this._warehouseRepository = warehouseRepository;
            this._supplierRepository = supplierRepository;
            this._supplierSiteRepository = supplierSiteRepository;
            this._materialDicRepository = materialDicRepository;
            this._ebsProjectRepository = ebsProjectRepository;
            this._ebsTaskRepository = ebsTaskRepository;
            this._employeeRepository = employeeRepository;
            this._organizationRepository = organizationRepository;
            this._areaRepository = areaRepository;
            this._logRecordService = logRecordService;
        }
        
        public async Task<ResponseResult> CreateEnterOrder(RequestEnterOrder requestEnterOrder)
        {
            ResponseResult responseResult=new ResponseResult();
            responseResult.Code = 200;
            try
            {
                Guard.Against.Null(requestEnterOrder,nameof(requestEnterOrder));
                Guard.Against.NullOrEmpty(requestEnterOrder.RequestEnterOrderRows,nameof(requestEnterOrder.RequestEnterOrderRows));
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                    LogDesc = string.Format("创建入库订单[{0}]",requestEnterOrder.DocumentNumber),
                    CreateTime = DateTime.Now
                });
                OUSpecification ouSpec= new OUSpecification(null,null,requestEnterOrder.OuCode,null);
                List<OU> ous = await this._ouRepository.ListAsync(ouSpec);
                if(ous.Count==0)throw new Exception(string.Format("业务实体编码[{0}],不存在!",requestEnterOrder.OuCode));
                OU ou = ous[0];
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,null,null,requestEnterOrder.OrganizationCode);
                List<Warehouse> warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                if(warehouses.Count==0)throw new Exception(string.Format("库存组织编码[{0}],不存在!",requestEnterOrder.OrganizationCode));
                Warehouse warehouse = warehouses[0];
                SupplierSpecification supplierSpec = new SupplierSpecification(Convert.ToInt32(requestEnterOrder.VendorId),null);
                List<Supplier> suppliers = await this._supplierRepository.ListAsync(supplierSpec);
                if(suppliers.Count==0)throw new Exception(string.Format("供应商头编号[{0}],不存在!",requestEnterOrder.VendorId));
                Supplier supplier = suppliers[0];
                SupplierSiteSpecification supplierSiteSpec = new SupplierSiteSpecification(Convert.ToInt32(requestEnterOrder.VendorSiteId),
                                                                                  null,null,null);
                List<SupplierSite> supplierSites = await this._supplierSiteRepository.ListAsync(supplierSiteSpec);
                if(supplierSites.Count==0)throw new Exception(string.Format("供应商地址编号[{0}],不存在!",requestEnterOrder.VendorSiteId));
                SupplierSite supplierSite = supplierSites[0];
                EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(Convert.ToInt32(requestEnterOrder.ItemId),null,
                    null,null,null,null,null);
                List<EBSProject> ebsProjects = await this._ebsProjectRepository.ListAsync(ebsProjectSpec);
                if(ebsProjects.Count==0)throw new Exception(string.Format("订单关联项目编号[{0}],不存在!",requestEnterOrder.ItemId));
                
                EmployeeSpecification employeeSpec = new EmployeeSpecification(Convert.ToInt32(requestEnterOrder.ManagerId),null,null,null);
                List<Employee> employees = await this._employeeRepository.ListAsync(employeeSpec);
                if(employees.Count==0)throw new Exception(string.Format("经办人编号[{0}],不存在!",requestEnterOrder.ManagerId));
                Order order = new Order
                { 
                    SourceId = Convert.ToInt32(requestEnterOrder.HeaderId),
                    OrderNumber = requestEnterOrder.DocumentNumber,
                    EmployeeId = Convert.ToInt32(requestEnterOrder.ManagerId),
                    OUId = ou.Id,
                    WarehouseId = warehouse.Id,
                    SupplierId = supplier.Id,
                    SupplierSiteId = supplierSite.Id,
                    BusinessTypeCode = requestEnterOrder.BusinessType,
                    Currency = requestEnterOrder.Currency,
                    TotalAmount = Convert.ToDouble(requestEnterOrder.TotalAmount),
                    ApplyTime = DateTime.Parse(requestEnterOrder.ExitEntryDate),
                    CreateTime = DateTime.Parse(requestEnterOrder.CreationDate),
                    EBSProjectId = Convert.ToInt32(requestEnterOrder.ItemId),
                    Memo = requestEnterOrder.Remark
                };
                List<OrderRow> orderRows = new List<OrderRow>();
                requestEnterOrder.RequestEnterOrderRows.ForEach(async (eor) =>
                {
                    MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(Convert.ToInt32(eor.MaterialId),
                                                                                 null,null,null,null);
                    List<MaterialDic> materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
                    if(materialDics.Count==0)throw new Exception(string.Format("物料编号[{0}],不存在!",eor.MaterialId));
                    MaterialDic materialDic = materialDics[0];
                    EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(Convert.ToInt32(eor.TaskId),null,null,
                        null,null,null,null);
                    List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                    if(ebsTasks.Count==0) throw new Exception(string.Format("订单行关联任务编号[{0}],不存在！",eor.TaskId));
                    EBSTask ebsTask = ebsTasks[0];
                    OrderRow orderRow = new OrderRow
                    {
                        SourceId = Convert.ToInt32(eor.LineId),
                        RowNumber = eor.LineNumber,
                        MaterialDicId = materialDic.Id,
                        PreCount = Convert.ToInt32(eor.ProcessingQuantity),
                        Price = Convert.ToInt32(eor.Price),
                        Amount = Convert.ToInt32(eor.Amount),
                        EBSTaskId = Convert.ToInt32(eor.TaskId),
                        Memo = eor.Remark
                    };
                    orderRows.Add(orderRow);
                });
                order.OrderRow = orderRows;
                var data = await this._orderService.CreateOrder(order);
                responseResult.Data = data;
            }
            catch (Exception ex)
            {
                responseResult.Code = 500;
                responseResult.Data = ex.Message;
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.Message,
                    CreateTime = DateTime.Now
                });
            }
            return responseResult;
        }

        public async Task<ResponseResult> CreateOutOrder(RequestOutOrder requestOutOrder)
        {
            ResponseResult responseResult = new ResponseResult();
            responseResult.Code = 200;
            try
            {
                Guard.Against.Null(requestOutOrder, nameof(requestOutOrder));
                Guard.Against.NullOrEmpty(requestOutOrder.RequestOutOrderRows,
                    nameof(requestOutOrder.RequestOutOrderRows));
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                    LogDesc = string.Format("创建领退料订单[{0}]",requestOutOrder.AlyNumber),
                    CreateTime = DateTime.Now
                });
                OUSpecification ouSpec = new OUSpecification(null, null, requestOutOrder.BusinessEntity, null);
                List<OU> ous = await this._ouRepository.ListAsync(ouSpec);
                if (ous.Count == 0)
                    throw new Exception(string.Format("业务实体编码[{0}],不存在!", requestOutOrder.BusinessEntity));
                OU ou = ous[0];
                WarehouseSpecification warehouseSpec =
                    new WarehouseSpecification(null, null, null, requestOutOrder.InventoryOrg);
                List<Warehouse> warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                if (warehouses.Count == 0)
                    throw new Exception(string.Format("库存组织编码[{0}],不存在!", requestOutOrder.InventoryOrg));
                Warehouse warehouse = warehouses[0];

                EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(
                    Convert.ToInt32(requestOutOrder.ItemId), null,
                    null, null, null, null, null);
                List<EBSProject> ebsProjects = await this._ebsProjectRepository.ListAsync(ebsProjectSpec);
                if (ebsProjects.Count == 0)
                    throw new Exception(string.Format("订单关联项目编号[{0}],不存在!", requestOutOrder.ItemId));
                EmployeeSpecification employeeSpec =
                    new EmployeeSpecification(Convert.ToInt32(requestOutOrder.CreationBy), null, null,null);
                List<Employee> employees = await this._employeeRepository.ListAsync(employeeSpec);
                if (employees.Count == 0)
                    throw new Exception(string.Format("经办人编号[{0}],不存在!", requestOutOrder.CreationBy));
                OrganizationSpecification organizationSpec =
                    new OrganizationSpecification(null, requestOutOrder.AlyDepCode, null, null);
                List<Organization> alyOrgs = await this._organizationRepository.ListAsync(organizationSpec);
                if (alyOrgs.Count == 0)
                    throw new Exception(string.Format("申请部门编码[{0}],不存在！", requestOutOrder.AlyDepCode));
                Organization alyOrg = alyOrgs[0];
                organizationSpec = new OrganizationSpecification(null, requestOutOrder.TransDepCode, null, null);
                List<Organization> transOrgs = await this._organizationRepository.ListAsync(organizationSpec);
                if (transOrgs.Count == 0)
                    throw new Exception(string.Format("领料部门编码[{0}],不存在！", requestOutOrder.TransDepCode));
                Organization transOrg = transOrgs[0];
                Order order = new Order
                {
                    SourceId = Convert.ToInt32(requestOutOrder.HeaderId),
                    OrderNumber = requestOutOrder.AlyNumber,
                    EmployeeId = Convert.ToInt32(requestOutOrder.CreationBy),
                    ApplyUserCode = requestOutOrder.AlyDepCode,
                    ApproveUserCode = requestOutOrder.TransDepCode,
                    OUId = ou.Id,
                    WarehouseId = warehouse.Id,
                    StatusTag = Convert.ToInt32(requestOutOrder.AlyStatusCode),
                    CallingParty = requestOutOrder.AplSourceCode,
                    BusinessTypeCode = requestOutOrder.BusinessTypeCode,
                    CreateTime = DateTime.Parse(requestOutOrder.CreationDate),
                    EBSProjectId = Convert.ToInt32(requestOutOrder.ItemId),
                    Memo = requestOutOrder.Remark
                };
                List<OrderRow> orderRows = new List<OrderRow>();
                requestOutOrder.RequestOutOrderRows.ForEach(async (eor) =>
                {
                    MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                        Convert.ToInt32(eor.MaterialId),
                        null, null, null, null);
                    List<MaterialDic> materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
                    if (materialDics.Count == 0)
                        throw new Exception(string.Format("物料编号[{0}],不存在!", eor.MaterialId));
                    MaterialDic materialDic = materialDics[0];
                    EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(Convert.ToInt32(eor.TaskId), null,
                        null,
                        null, null, null, null);
                    List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                    if (ebsTasks.Count == 0) throw new Exception(string.Format("订单行关联任务编号[{0}],不存在！", eor.TaskId));
                    EBSTask ebsTask = ebsTasks[0];
                    ReservoirAreaSpecification reservoirAreaSpec =
                        new ReservoirAreaSpecification(null, eor.InventoryCode, null, null, null, null);
                    List<ReservoirArea> areas = await this._areaRepository.ListAsync(reservoirAreaSpec);
                    if (areas.Count == 0) throw new Exception(string.Format("子库存编码[{0}],不存在！", eor.InventoryCode));
                    ReservoirArea area = areas[0];
                    OrderRow orderRow = new OrderRow
                    {
                        SourceId = Convert.ToInt32(eor.LineId),
                        RowNumber = eor.LineNum,
                        MaterialDicId = materialDic.Id,
                        UseFor = eor.UseFor,
                        PreCount = Convert.ToInt32(eor.ReqQty),
                        CancelCount = Convert.ToInt32(eor.CancelQty),
                        ReservoirAreaId = area.Id,
                        EBSTaskId = Convert.ToInt32(eor.TaskId),
                        Memo = eor.Remark
                    };
                    orderRows.Add(orderRow);
                });
                order.OrderRow = orderRows;
                var data = await this._orderService.CreateOrder(order);
                responseResult.Data = data;
            }
            catch (Exception ex)
            {
                responseResult.Code = 500;
                responseResult.Data = ex.Message;
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.Message,
                    CreateTime = DateTime.Now
                });
            }

            return responseResult;

        }
    }
}