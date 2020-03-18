using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.BasicInformation;
using Web.WebServices.Interfaces;
using Web.WebServices.Models;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using ApplicationCore.Entities.FlowRecord;
using Ardalis.GuardClauses;

namespace Web.WebServices.Services
{
    public class OrderSOAPService:IOrderSOAPService
    {
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
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;

        public OrderSOAPService(IAsyncRepository<OU> ouRepository,
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
                                ILogRecordService logRecordService,
                                IAsyncRepository<OrderRow> orderRowRepository)
        {
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
            this._orderRowRepository = orderRowRepository;
        }

        public async Task<ResponseResult> CreateRKJSOrder(List<RequestRKJSOrder> RequestRKJSOrders,bool bulkTransaction)
        {
            Guard.Against.Null(RequestRKJSOrders, nameof(RequestRKJSOrders));

            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                ResponseResult responseResult = new ResponseResult();
                responseResult.Code = 200;
                try
                {
                    foreach (var RequestRKJSOrder in RequestRKJSOrders)
                    {
                        OrderSpecification orderSpec = new OrderSpecification(null, RequestRKJSOrder.DocumentNumber,
                            null, null, null, null, null, null, null,
                            null, null, null, null, null,
                            null, null, null, null, null);
                        List<Order> orders = await this._orderRepository.ListAsync(orderSpec);

                        OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null,
                            RequestRKJSOrder.DocumentNumber, null, null, null, null, null);
                        List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);

                        OUSpecification ouSpec = new OUSpecification(null, null, RequestRKJSOrder.OuCode, null);
                        List<OU> ous = await this._ouRepository.ListAsync(ouSpec);
                        if (ous.Count == 0)
                        {
                            string err = string.Format("入库订单[{0}],关联业务实体编码[{1}]不存在!",
                                RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.OuCode);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestRKJSOrder.Result = "-1";
                                RequestRKJSOrder.ErrMsg = err;
                                continue;
                            }
                        }

                        OU ou = ous[0];
                        WarehouseSpecification warehouseSpec =
                            new WarehouseSpecification(null, null, null, RequestRKJSOrder.OrganizationCode);
                        List<Warehouse> warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                        if (warehouses.Count == 0)
                        {
                            string err = string.Format("入库订单[{0}],关联库存组织编码[{1}]不存在!",
                                RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.OrganizationCode);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestRKJSOrder.Result = "-1";
                                RequestRKJSOrder.ErrMsg = err;
                                continue;
                            }
                        }

                        Warehouse warehouse = warehouses[0];
                        SupplierSpecification supplierSpec =
                            new SupplierSpecification(Convert.ToInt32(RequestRKJSOrder.VendorId), null);
                        List<Supplier> suppliers = await this._supplierRepository.ListAsync(supplierSpec);
                        if (suppliers.Count == 0)
                        {
                            string err = string.Format("入库订单[{0}],关联供应商头Id[{1}]不存在!",
                                RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.VendorId);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestRKJSOrder.Result = "-1";
                                RequestRKJSOrder.ErrMsg = err;
                                continue;
                            }
                        }

                        Supplier supplier = suppliers[0];
                        SupplierSiteSpecification supplierSiteSpec = new SupplierSiteSpecification(
                            Convert.ToInt32(RequestRKJSOrder.VendorSiteId),
                            null, null, null);
                        List<SupplierSite> supplierSites =
                            await this._supplierSiteRepository.ListAsync(supplierSiteSpec);
                        if (supplierSites.Count == 0)
                        {
                            string err = string.Format("入库订单[{0}],关联供应商地址Id[{1}]不存在!",
                                RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.VendorSiteId);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestRKJSOrder.Result = "-1";
                                RequestRKJSOrder.ErrMsg = err;
                                continue;
                            }
                        }

                        SupplierSite supplierSite = supplierSites[0];
                        EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(
                            Convert.ToInt32(RequestRKJSOrder.ItemId), null,
                            null, null, null, null, null);
                        List<EBSProject> ebsProjects = await this._ebsProjectRepository.ListAsync(ebsProjectSpec);
                        if (ebsProjects.Count == 0)
                        {
                            string err = string.Format("入库订单[{0}],关联项目Id[{0}]不存在!", RequestRKJSOrder.DocumentNumber,
                                RequestRKJSOrder.ItemId);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestRKJSOrder.Result = "-1";
                                RequestRKJSOrder.ErrMsg = err;
                                continue;
                            }
                        }

                        EmployeeSpecification employeeSpec =
                            new EmployeeSpecification(Convert.ToInt32(RequestRKJSOrder.ManagerId), null, null, null);
                        List<Employee> employees = await this._employeeRepository.ListAsync(employeeSpec);
                        if (employees.Count == 0)
                        {
                            string err = string.Format("入库订单[{0}],关联经办人Id[{1}]不存在!",
                                RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.ManagerId);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestRKJSOrder.Result = "-1";
                                RequestRKJSOrder.ErrMsg = err;
                                continue;
                            }
                        }

                        if (orders.Count > 0)
                        {
                            var srcOrder = orders[0];
                            if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                            {
                                string err = string.Format("入库订单[{0}]已经完成无法修改！", RequestRKJSOrder.DocumentNumber);
                                if (bulkTransaction)
                                    throw new Exception(err);
                                else
                                {
                                    responseResult.Code = 300;
                                    RequestRKJSOrder.Result = "-1";
                                    RequestRKJSOrder.ErrMsg = err;
                                    continue;
                                }
                            }

                            if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                            {
                                string err = string.Format("入库订单[{0}]已经关闭无法修改！", RequestRKJSOrder.DocumentNumber);
                                if (bulkTransaction)
                                    throw new Exception(err);
                                else
                                {
                                    responseResult.Code = 300;
                                    RequestRKJSOrder.Result = "-1";
                                    RequestRKJSOrder.ErrMsg = err;
                                    continue;
                                }
                            }
                            
                            List<OrderRow> addOrderRows = new List<OrderRow>();
                            foreach (var eor in RequestRKJSOrder.RequestRKJSRows)
                            {
                                var existRow = orderRows.Find(r => r.RowNumber == eor.LineNumber);
                                if (existRow == null)
                                {
                                    MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                        Convert.ToInt32(eor.MaterialId),
                                        null, null, null, null);
                                    List<MaterialDic> materialDics =
                                        await this._materialDicRepository.ListAsync(materialDicSpec);
                                    if (materialDics.Count == 0)
                                    {
                                        string err = string.Format("入库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                            RequestRKJSOrder.DocumentNumber, eor.LineNumber, eor.MaterialId);
                                        if (bulkTransaction)
                                            throw new Exception(err);
                                        else
                                        {
                                            responseResult.Code = 301;
                                            RequestRKJSOrder.Result = "-1";
                                            eor.Result = "-1";
                                            eor.ErrMsg = err;
                                            continue;
                                        }
                                    }

                                    MaterialDic materialDic = materialDics[0];
                                    EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(
                                        Convert.ToInt32(eor.TaskId),
                                        null, null,
                                        null, null, null, null);
                                    List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                                    if (ebsTasks.Count == 0)
                                    {
                                        string err = string.Format("入库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                            RequestRKJSOrder.DocumentNumber, eor.LineNumber, eor.TaskId);
                                        if (bulkTransaction)
                                            throw new Exception(err);
                                        else
                                        {
                                            responseResult.Code = 301;
                                            RequestRKJSOrder.Result = "-1";
                                            eor.Result = "-1";
                                            eor.ErrMsg = err;
                                            continue;
                                        }
                                    }
                                    
                                    EBSTask ebsTask = ebsTasks[0];
                                    OrderRow addOrderRow = new OrderRow
                                    {
                                        OrderId = srcOrder.Id,
                                        SourceId = Convert.ToInt32(eor.LineId),
                                        RowNumber = eor.LineNumber,
                                        MaterialDicId = materialDic.Id,
                                        PreCount = Convert.ToInt32(eor.ProcessingQuantity),
                                        Price = Convert.ToInt32(eor.Price),
                                        Amount = Convert.ToInt32(eor.Amount),
                                        EBSTaskId = ebsTask.Id,
                                        Memo = eor.Remark
                                    };
                                    addOrderRows.Add(addOrderRow);
                                }
                                
                            }
                            
                            if (RequestRKJSOrder.Result == "-1")
                            {
                                continue;
                            }
                            else
                            {
                                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    try
                                    {
                                        this._orderRowRepository.Add(addOrderRows);
                                        scope.Complete();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                                StringBuilder sb = new StringBuilder(string.Format("修改入库订单[{0}]\n", srcOrder.Id));
                                if (addOrderRows.Count > 0)
                                    sb.Append(string.Format("新增入库订单行[{0}]\n",
                                        string.Join(',', addOrderRows.ConvertAll(r => r.Id))));

                                await this._logRecordService.AddLog(new LogRecord
                                {
                                    LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                    LogDesc = sb.ToString(),
                                    CreateTime = DateTime.Now
                                });
                            }
                        }
                        else
                        {
                            Order addOrder = new Order
                            {
                                SourceId = Convert.ToInt32(RequestRKJSOrder.HeaderId),
                                OrderNumber = RequestRKJSOrder.DocumentNumber,
                                EmployeeId = Convert.ToInt32(RequestRKJSOrder.ManagerId),
                                OUId = ou.Id,
                                OrderTypeId = Convert.ToInt32(RequestRKJSOrder.DocumentType),
                                WarehouseId = warehouse.Id,
                                SupplierId = supplier.Id,
                                SupplierSiteId = supplierSite.Id,
                                BusinessTypeCode = RequestRKJSOrder.BusinessType,
                                Currency = RequestRKJSOrder.Currency,
                                TotalAmount = Convert.ToDouble(RequestRKJSOrder.TotalAmount),
                                ApplyTime = DateTime.Parse(RequestRKJSOrder.ExitEntryDate),
                                CreateTime = DateTime.Parse(RequestRKJSOrder.CreationDate),
                                EBSProjectId = Convert.ToInt32(RequestRKJSOrder.ItemId),
                                Memo = RequestRKJSOrder.Remark
                            };
                            List<OrderRow> addOrderRows = new List<OrderRow>();
                            foreach (var eor in RequestRKJSOrder.RequestRKJSRows)
                            {
                                MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                    Convert.ToInt32(eor.MaterialId),
                                    null, null, null, null);
                                List<MaterialDic> materialDics =
                                    await this._materialDicRepository.ListAsync(materialDicSpec);
                                if (materialDics.Count == 0)
                                {
                                    string err = string.Format("入库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                        RequestRKJSOrder.DocumentNumber, eor.LineNumber, eor.MaterialId);
                                    if (bulkTransaction)
                                        throw new Exception(err);
                                    else
                                    {
                                        responseResult.Code = 301;
                                        RequestRKJSOrder.Result = "-1";
                                        eor.Result = "-1";
                                        eor.ErrMsg = err;
                                        continue;
                                    }
                                }

                                MaterialDic materialDic = materialDics[0];
                                EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(Convert.ToInt32(eor.TaskId),
                                    null, null,
                                    null, null, null, null);
                                List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                                if (ebsTasks.Count == 0)
                                {
                                    string err = string.Format("入库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                        RequestRKJSOrder.DocumentNumber, eor.LineNumber, eor.TaskId);
                                    if (bulkTransaction)
                                        throw new Exception(err);
                                    else
                                    {
                                        responseResult.Code = 301;
                                        RequestRKJSOrder.Result = "-1";
                                        eor.Result = "-1";
                                        eor.ErrMsg = err;
                                        continue;
                                    }
                                }
                                
                                if (eor.Result == "-1") continue;
                                EBSTask ebsTask = ebsTasks[0];
                                OrderRow addOrderRow = new OrderRow
                                {
                                    SourceId = Convert.ToInt32(eor.LineId),
                                    RowNumber = eor.LineNumber,
                                    MaterialDicId = materialDic.Id,
                                    PreCount = Convert.ToInt32(eor.ProcessingQuantity),
                                    Price = Convert.ToInt32(eor.Price),
                                    Amount = Convert.ToInt32(eor.Amount),
                                    EBSTaskId = ebsTask.Id,
                                    Memo = eor.Remark
                                };
                                addOrderRows.Add(addOrderRow);
                            }

                            if (RequestRKJSOrder.Result == "-1")
                            {
                                continue;
                            }
                            else
                            {
                                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    try
                                    {
                                        Order saveOrder = this._orderRepository.Add(addOrder);
                                        addOrderRows.ForEach(om => om.OrderId = saveOrder.Id);
                                        this._orderRowRepository.Add(addOrderRows);
                                        scope.Complete();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }

                                await this._logRecordService.AddLog(new LogRecord
                                {
                                    LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                    LogDesc = string.Format("新增入库订单[{0}]\n新增入库订单行[{1}]", addOrder.Id,
                                        string.Join(',', addOrderRows.ConvertAll(r => r.Id))),
                                    CreateTime = DateTime.Now
                                });
                            }
                        }

                    }

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
                    return responseResult;
                }

                responseResult.Data = RequestRKJSOrders;
                return responseResult;
            }
        }

        public async Task<ResponseResult> CreateCKLLOrder(List<RequestCKLLOrder> RequestCKLLOrders,bool bulkTransaction)
        {
            Guard.Against.Null(RequestCKLLOrders, nameof(RequestCKLLOrders));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                ResponseResult responseResult = new ResponseResult();
                responseResult.Code = 200;
                try
                {
                    foreach (var RequestCKLLOrder in RequestCKLLOrders)
                    {
                        OrderSpecification orderSpec = new OrderSpecification(null, RequestCKLLOrder.AlyNumber, null,
                            null,
                            null, null, null, null, null, null, null, null, null, null,
                            null, null, null, null, null);
                        List<Order> orders = await this._orderRepository.ListAsync(orderSpec);

                        OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null,
                            RequestCKLLOrder.AlyNumber, null, null, null, null, null);
                        List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);

                        OUSpecification ouSpec = new OUSpecification(null, null, RequestCKLLOrder.BusinessEntity, null);
                        List<OU> ous = await this._ouRepository.ListAsync(ouSpec);
                        if (ous.Count == 0)
                        {
                            string err = string.Format("出库订单[{0}],关联业务实体编码[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                           RequestCKLLOrder.BusinessEntity);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestCKLLOrder.Result = "-1";
                                RequestCKLLOrder.ErrMsg = err;
                                continue;
                            }
                        }
                        OU ou = ous[0];
                        WarehouseSpecification warehouseSpec =
                            new WarehouseSpecification(null, null, null, RequestCKLLOrder.InventoryOrg);
                        List<Warehouse> warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                        if (warehouses.Count == 0)
                        {
                            string err = string.Format("出库订单[{0}],关联库存组织编码[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                RequestCKLLOrder.InventoryOrg);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestCKLLOrder.Result = "-1";
                                RequestCKLLOrder.ErrMsg = err;
                                continue;
                            }
                        }

                        Warehouse warehouse = warehouses[0];

                        EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(
                            Convert.ToInt32(RequestCKLLOrder.ItemId), null,
                            null, null, null, null, null);
                        List<EBSProject> ebsProjects = await this._ebsProjectRepository.ListAsync(ebsProjectSpec);
                        if (ebsProjects.Count == 0)
                        {
                            string err = string.Format("出库订单[{0}],关联项目Id[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                RequestCKLLOrder.ItemId);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestCKLLOrder.Result = "-1";
                                RequestCKLLOrder.ErrMsg = err;
                                continue;
                            }
                        }
                        EmployeeSpecification employeeSpec =
                            new EmployeeSpecification(Convert.ToInt32(RequestCKLLOrder.CreationBy), null, null, null);
                        List<Employee> employees = await this._employeeRepository.ListAsync(employeeSpec);
                        if (employees.Count == 0)
                        {
                            string err = string.Format("出库订单[{0}],关联经办人Id[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                RequestCKLLOrder.CreationBy);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestCKLLOrder.Result = "-1";
                                RequestCKLLOrder.ErrMsg = err;
                                continue;
                            }
                        }
                        OrganizationSpecification organizationSpec =
                            new OrganizationSpecification(null, RequestCKLLOrder.AlyDepCode, null, null);
                        List<Organization> alyOrgs = await this._organizationRepository.ListAsync(organizationSpec);
                        if (alyOrgs.Count == 0)
                        {
                            string err = string.Format("出库订单[{0}],关联申请部门编码[{1}]不存在！", RequestCKLLOrder.AlyNumber,
                                RequestCKLLOrder.AlyDepCode);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestCKLLOrder.Result = "-1";
                                RequestCKLLOrder.ErrMsg = err;
                                continue;
                            }
                        }
                        Organization alyOrg = alyOrgs[0];
                        organizationSpec =
                            new OrganizationSpecification(null, RequestCKLLOrder.TransDepCode, null, null);
                        List<Organization> transOrgs = await this._organizationRepository.ListAsync(organizationSpec);
                        if (transOrgs.Count == 0)
                        {
                            string err = string.Format("出库订单[{0}],关联领料部门编码[{1}]不存在！", RequestCKLLOrder.AlyNumber,
                                RequestCKLLOrder.TransDepCode);
                            if (bulkTransaction)
                                throw new Exception(err);
                            else
                            {
                                responseResult.Code = 300;
                                RequestCKLLOrder.Result = "-1";
                                RequestCKLLOrder.ErrMsg = err;
                                continue;
                            }
                        }
                        
                        if (orders.Count > 0)
                        {
                            var srcOrder = orders[0];
                            if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                            {
                                string err = string.Format("出库订单[{0}]已经完成无法修改！", RequestCKLLOrder.AlyNumber);
                                if (bulkTransaction)
                                    throw new Exception(err);
                                else
                                {
                                    responseResult.Code = 300;
                                    RequestCKLLOrder.Result = "-1";
                                    RequestCKLLOrder.ErrMsg = err;
                                    continue;
                                }
                                
                            }
                            if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                            {
                                string err = string.Format("出库订单[{0}]已经关闭无法修改！", RequestCKLLOrder.AlyNumber);
                                if (bulkTransaction)
                                    throw new Exception(err);
                                else
                                {
                                    responseResult.Code = 300;
                                    RequestCKLLOrder.Result = "-1";
                                    RequestCKLLOrder.ErrMsg = err;
                                    continue;
                                }
                                
                            }
                            if (RequestCKLLOrder.AlyStatusCode == "3")
                            {
                                if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.执行中)||
                                    srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成)||
                                    srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                {
                                    string err= string.Format("出库订单[{0}][{1}]无法关闭！", RequestCKLLOrder.AlyNumber,
                                        Enum.GetName(typeof(ORDER_STATUS),srcOrder.Status));
                                    if (bulkTransaction)
                                        throw new Exception(err);
                                    else
                                    {
                                        responseResult.Code = 300;
                                        RequestCKLLOrder.Result = "-1";
                                        RequestCKLLOrder.ErrMsg = err;
                                        continue;
                                    }
                                }
                                
                                List<Order> updOrders = new List<Order>();
                                List<OrderRow> updOrderRows = new List<OrderRow>();

                                List<OrderRow> queueExeRows = orderRows.Where(r =>
                                    r.Status == Convert.ToInt32(ORDER_STATUS.待处理)).ToList();
                                queueExeRows.ForEach(r => r.Status = Convert.ToInt32(ORDER_STATUS.关闭));
                                srcOrder.Status = Convert.ToInt32(ORDER_STATUS.关闭);
                                updOrders.Add(srcOrder);
                                updOrderRows.AddRange(queueExeRows);

                                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    try
                                    {
                                        this._orderRepository.Update(updOrders);
                                        this._orderRowRepository.Update(updOrderRows);
                                        scope.Complete();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }

                                await this._logRecordService.AddLog(new LogRecord
                                {
                                    LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                    LogDesc = "关闭出库订单[{0}]" + srcOrder.OrderNumber,
                                    CreateTime = DateTime.Now
                                });

                            }
                            else
                            {
                                List<OrderRow> addOrderRows = new List<OrderRow>();
                                List<OrderRow> updOrderRows = new List<OrderRow>();
                                foreach (var eor in RequestCKLLOrder.RequestCKLLRows)
                                {
                                    var existRow = orderRows.Find(r => r.RowNumber == eor.LineNum);
                                    if (existRow == null)
                                    {
                                        MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                            Convert.ToInt32(eor.MaterialId),
                                            null, null, null, null);
                                        List<MaterialDic> materialDics =
                                            await this._materialDicRepository.ListAsync(materialDicSpec);
                                        if (materialDics.Count == 0)
                                        {
                                            string err = string.Format("出库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                                RequestCKLLOrder.AlyNumber, eor.LineNum, eor.MaterialId);
                                            if (bulkTransaction)
                                                throw new Exception(err);
                                            else
                                            {
                                                responseResult.Code = 301;
                                                RequestCKLLOrder.Result = "-1";
                                                eor.Result = "-1";
                                                eor.ErrMsg = err;
                                                continue;
                                            }
                                        }
                                        MaterialDic materialDic = materialDics[0];
                                        EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(
                                            Convert.ToInt32(eor.TaskId), null,
                                            null,
                                            null, null, null, null);
                                        List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                                        if (ebsTasks.Count == 0)
                                        {
                                            string err = string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                                RequestCKLLOrder.AlyNumber, eor.LineNum, eor.TaskId);
                                            if (bulkTransaction)
                                                throw new Exception(err);
                                            else
                                            {
                                                responseResult.Code = 301;
                                                RequestCKLLOrder.Result = "-1";
                                                eor.Result = "-1";
                                                eor.ErrMsg = err;
                                                continue;
                                            }
                                        }
                                        
                                        ReservoirAreaSpecification reservoirAreaSpec =
                                            new ReservoirAreaSpecification(null, eor.InventoryCode, null, null, null,
                                                null);
                                        List<ReservoirArea> areas =
                                            await this._areaRepository.ListAsync(reservoirAreaSpec);
                                        if (areas.Count == 0)
                                        {
                                            string err = string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                                RequestCKLLOrder.AlyNumber, eor.LineNum, eor.InventoryCode);
                                            if (bulkTransaction)
                                                throw new Exception(err);
                                            else
                                            {
                                                responseResult.Code = 301;
                                                RequestCKLLOrder.Result = "-1";
                                                eor.Result = "-1";
                                                eor.ErrMsg = err;
                                                continue;
                                            }
                                        }
                                       
                                        ReservoirArea area = areas[0];
                                        OrderRow addOrderRow = new OrderRow
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
                                        addOrderRows.Add(addOrderRow);
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(eor.CancelQty) > (existRow.PreCount - existRow.Sorting))
                                        {
                                            string err = string.Format(
                                                "修改出库订单[{0}],订单行[{1}],取消数量大于剩余数量,已出库[{2}],剩余[{3}]",
                                                RequestCKLLOrder.AlyNumber, eor.LineNum, existRow.Sorting,
                                                existRow.PreCount - existRow.Sorting);
                                            if (bulkTransaction)
                                                throw new Exception(err);
                                            else
                                            {
                                                responseResult.Code = 301;
                                                RequestCKLLOrder.Result = "-1";
                                                eor.Result = "-1";
                                                eor.ErrMsg = err;
                                                continue;
                                            }
                                        }
                                       
                                        existRow.CancelCount = Convert.ToInt32(eor.CancelQty);
                                        existRow.PreCount = Convert.ToInt32(eor.ReqQty);
                                        updOrderRows.Add(existRow);
                                    }

                                    if (RequestCKLLOrder.Result == "-1")
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            try
                                            {
                                                this._orderRowRepository.Add(addOrderRows);
                                                this._orderRowRepository.Update(updOrderRows);
                                                scope.Complete();
                                            }
                                            catch (Exception ex)
                                            {
                                                throw ex;
                                            }
                                        }

                                        StringBuilder sb =
                                            new StringBuilder(string.Format("修改出库订单[{0}]\n", srcOrder.Id));
                                        if (addOrderRows.Count > 0)
                                            sb.Append(string.Format("新增出库订单行[{0}]\n",
                                                string.Join(',', addOrderRows.ConvertAll(r => r.Id))));
                                        if (updOrderRows.Count > 0)
                                            sb.Append(string.Format("修改出库订单行[{0}]\n",
                                                string.Join(',', updOrderRows.ConvertAll(r => r.Id))));

                                        await this._logRecordService.AddLog(new LogRecord
                                        {
                                            LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                            LogDesc = sb.ToString(),
                                            CreateTime = DateTime.Now
                                        });
                                    }

                                }
                            }

                        }
                        else
                        {
                            Order addOrder = new Order
                            {
                                SourceId = Convert.ToInt32(RequestCKLLOrder.HeaderId),
                                OrderNumber = RequestCKLLOrder.AlyNumber,
                                EmployeeId = Convert.ToInt32(RequestCKLLOrder.CreationBy),
                                ApplyUserCode = RequestCKLLOrder.AlyDepCode,
                                ApproveUserCode = RequestCKLLOrder.TransDepCode,
                                OUId = ou.Id,
                                OrderTypeId = Convert.ToInt32(ORDER_TYPE.出库领料),
                                WarehouseId = warehouse.Id,
                                CallingParty = RequestCKLLOrder.AplSourceCode,
                                BusinessTypeCode = RequestCKLLOrder.BusinessTypeCode,
                                CreateTime = DateTime.Parse(RequestCKLLOrder.CreationDate),
                                EBSProjectId = Convert.ToInt32(RequestCKLLOrder.ItemId),
                                Memo = RequestCKLLOrder.Remark
                            };
                            List<OrderRow> addOrderRows = new List<OrderRow>();
                            foreach (var eor in  RequestCKLLOrder.RequestCKLLRows)
                            {
                                MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                    Convert.ToInt32(eor.MaterialId),
                                    null, null, null, null);
                                List<MaterialDic> materialDics =
                                    await this._materialDicRepository.ListAsync(materialDicSpec);
                                if (materialDics.Count == 0)
                                {
                                    string err = string.Format("出库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                        RequestCKLLOrder.AlyNumber, eor.LineNum, eor.MaterialId);
                                    if (bulkTransaction)
                                        throw new Exception(err);
                                    else
                                    {
                                        responseResult.Code = 301;
                                        RequestCKLLOrder.Result = "-1";
                                        eor.Result = "-1";
                                        eor.ErrMsg = err;
                                        continue;
                                    }
                                }
                                MaterialDic materialDic = materialDics[0];
                                EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(Convert.ToInt32(eor.TaskId),
                                    null,
                                    null,
                                    null, null, null, null);
                                List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                                if (ebsTasks.Count == 0)
                                {
                                    string err = string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                        RequestCKLLOrder.AlyNumber, eor.LineNum, eor.TaskId);
                                    if (bulkTransaction)
                                        throw new Exception(err);
                                    else
                                    {
                                        responseResult.Code = 301;
                                        RequestCKLLOrder.Result = "-1";
                                        eor.Result = "-1";
                                        eor.ErrMsg = err;
                                        continue;
                                    }
                                }
                                
                                ReservoirAreaSpecification reservoirAreaSpec =
                                    new ReservoirAreaSpecification(null, eor.InventoryCode, null, null, null, null);
                                List<ReservoirArea> areas = await this._areaRepository.ListAsync(reservoirAreaSpec);
                                if (areas.Count == 0)
                                {
                                    string err = string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                        RequestCKLLOrder.AlyNumber, eor.LineNum, eor.InventoryCode);
                                    if (bulkTransaction)
                                        throw new Exception(err);
                                    else
                                    {
                                        responseResult.Code = 301;
                                        RequestCKLLOrder.Result = "-1";
                                        eor.Result = "-1";
                                        eor.ErrMsg = err;
                                        continue;
                                    }
                                }
                                
                                ReservoirArea area = areas[0];
                                OrderRow addOrderRow = new OrderRow
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
                                addOrderRows.Add(addOrderRow);
                            }

                            if (RequestCKLLOrder.Result == "-1")
                            {
                                continue;
                            }
                            else
                            {
                                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    try
                                    {
                                        Order saveOrder = this._orderRepository.Add(addOrder);
                                        addOrderRows.ForEach(om => om.OrderId = saveOrder.Id);
                                        this._orderRowRepository.Add(addOrderRows);
                                        scope.Complete();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }

                                await this._logRecordService.AddLog(new LogRecord
                                {
                                    LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                    LogDesc = string.Format("新增入库订单[{0}]\n新增入库订单行[{1}]", addOrder.Id,
                                        string.Join(',', addOrderRows.ConvertAll(r => r.Id))),
                                    CreateTime = DateTime.Now
                                });

                            }

                        }

                    }

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
                    return responseResult;
                }

                responseResult.Data = RequestCKLLOrders;
                return responseResult;
            }

        }
    }
}