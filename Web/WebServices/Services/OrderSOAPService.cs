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

        public async Task<ResponseResult> CreateEnterOrder(RequestEnterOrder requestEnterOrder)
        {
            Guard.Against.Null(requestEnterOrder, nameof(requestEnterOrder));
            Guard.Against.NullOrEmpty(requestEnterOrder.RequestEnterOrderRows,
                nameof(requestEnterOrder.RequestEnterOrderRows));

            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                ResponseResult responseResult = new ResponseResult();
                responseResult.Code = 200;
                try
                {
                    OrderSpecification orderSpec = new OrderSpecification(null, requestEnterOrder.DocumentNumber, null,
                        null,
                        null, null, null, null, null, null, null,
                        null, null, null, null, null);
                    List<Order> orders = await this._orderRepository.ListAsync(orderSpec);

                    OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null,
                        requestEnterOrder.DocumentNumber, null, null, null, null, null);
                    List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);

                    OUSpecification ouSpec = new OUSpecification(null, null, requestEnterOrder.OuCode, null);
                    List<OU> ous = await this._ouRepository.ListAsync(ouSpec);
                    if (ous.Count == 0)
                        throw new Exception(string.Format("入库订单[{0}],关联业务实体编码[{1}]不存在!",
                            requestEnterOrder.DocumentNumber, requestEnterOrder.OuCode));
                    OU ou = ous[0];
                    WarehouseSpecification warehouseSpec =
                        new WarehouseSpecification(null, null, null, requestEnterOrder.OrganizationCode);
                    List<Warehouse> warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                    if (warehouses.Count == 0)
                        throw new Exception(string.Format("入库订单[{0}],关联库存组织编码[{1}]不存在!",
                            requestEnterOrder.DocumentNumber, requestEnterOrder.OrganizationCode));
                    Warehouse warehouse = warehouses[0];
                    SupplierSpecification supplierSpec =
                        new SupplierSpecification(Convert.ToInt32(requestEnterOrder.VendorId), null);
                    List<Supplier> suppliers = await this._supplierRepository.ListAsync(supplierSpec);
                    if (suppliers.Count == 0)
                        throw new Exception(string.Format("入库订单[{0}],关联供应商头Id[{1}]不存在!",
                            requestEnterOrder.DocumentNumber, requestEnterOrder.VendorId));
                    Supplier supplier = suppliers[0];
                    SupplierSiteSpecification supplierSiteSpec = new SupplierSiteSpecification(
                        Convert.ToInt32(requestEnterOrder.VendorSiteId),
                        null, null, null);
                    List<SupplierSite> supplierSites = await this._supplierSiteRepository.ListAsync(supplierSiteSpec);
                    if (supplierSites.Count == 0)
                        throw new Exception(string.Format("入库订单[{0}],关联供应商地址Id[{1}]不存在!",
                            requestEnterOrder.DocumentNumber, requestEnterOrder.VendorSiteId));
                    SupplierSite supplierSite = supplierSites[0];
                    EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(
                        Convert.ToInt32(requestEnterOrder.ItemId), null,
                        null, null, null, null, null);
                    List<EBSProject> ebsProjects = await this._ebsProjectRepository.ListAsync(ebsProjectSpec);
                    if (ebsProjects.Count == 0)
                        throw new Exception(string.Format("入库订单[{0}],关联项目Id[{0}]不存在!", requestEnterOrder.DocumentNumber,
                            requestEnterOrder.ItemId));

                    EmployeeSpecification employeeSpec =
                        new EmployeeSpecification(Convert.ToInt32(requestEnterOrder.ManagerId), null, null, null);
                    List<Employee> employees = await this._employeeRepository.ListAsync(employeeSpec);
                    if (employees.Count == 0)
                        throw new Exception(string.Format("入库订单[{0}],关联经办人Id[{1}]不存在!",
                            requestEnterOrder.DocumentNumber, requestEnterOrder.ManagerId));

                    if (orders.Count > 0)
                    {
                        var srcOrder = orders[0];
                        if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                            throw new Exception(string.Format("入库订单[{0}]已经完成无法修改！", requestEnterOrder.DocumentNumber));
                        List<OrderRow> addOrderRows = new List<OrderRow>();
                        List<OrderRow> updOrderRows = new List<OrderRow>();
                        requestEnterOrder.RequestEnterOrderRows.ForEach(async (eor) =>
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
                                    throw new Exception(string.Format("入库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                        requestEnterOrder.DocumentNumber, eor.LineNumber, eor.MaterialId));
                                MaterialDic materialDic = materialDics[0];
                                EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(
                                    Convert.ToInt32(eor.TaskId),
                                    null, null,
                                    null, null, null, null);
                                List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                                if (ebsTasks.Count == 0)
                                    throw new Exception(string.Format("入库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                        requestEnterOrder.DocumentNumber, eor.LineNumber, eor.TaskId));
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
                            else
                            {
                                if (Convert.ToInt32(eor.ProcessingQuantity) < existRow.PreCount&&
                                    existRow.Status!=Convert.ToInt32(ORDER_STATUS.完成))
                                {
                                    if (Convert.ToInt32(eor.ProcessingQuantity) <
                                        (existRow.PreCount - existRow.Sorting))
                                        throw new Exception(string.Format(
                                            "修改入库订单[{0}],订单行[{1}],修改数量大于剩余数量,已分拣[{2}],剩余[{3}]",
                                            requestEnterOrder.DocumentNumber, eor.LineNumber, existRow.Sorting,
                                            existRow.PreCount - existRow.Sorting));
                                    existRow.PreCount = Convert.ToInt32(eor.ProcessingQuantity);
                                    updOrderRows.Add(existRow);
                                }
                            }
                        });
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
                        StringBuilder sb = new StringBuilder(string.Format("修改入库订单[{0}]\n",srcOrder.Id));
                        if (addOrderRows.Count > 0)
                            sb.Append(string.Format("新增入库订单行[{0}]\n",
                                string.Join(',', addOrderRows.ConvertAll(r => r.Id))));
                        if (updOrderRows.Count > 0)
                            sb.Append(string.Format("修改入库订单行[{0}]\n",
                                string.Join(',', updOrderRows.ConvertAll(r => r.Id))));
                                
                        await this._logRecordService.AddLog(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                            LogDesc = sb.ToString(),
                            CreateTime = DateTime.Now
                        });
                    }
                    else
                    {
                        Order addOrder = new Order
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
                        List<OrderRow> addOrderRows = new List<OrderRow>();
                        requestEnterOrder.RequestEnterOrderRows.ForEach(async (eor) =>
                        {
                            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                Convert.ToInt32(eor.MaterialId),
                                null, null, null, null);
                            List<MaterialDic> materialDics =
                                await this._materialDicRepository.ListAsync(materialDicSpec);
                            if (materialDics.Count == 0)
                                throw new Exception(string.Format("入库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                    requestEnterOrder.DocumentNumber, eor.LineNumber, eor.MaterialId));
                            MaterialDic materialDic = materialDics[0];
                            EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(Convert.ToInt32(eor.TaskId),
                                null, null,
                                null, null, null, null);
                            List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                            if (ebsTasks.Count == 0)
                                throw new Exception(string.Format("入库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                    requestEnterOrder.DocumentNumber, eor.LineNumber, eor.TaskId));
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
                        });
                       
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

        public async Task<ResponseResult> CreateOutOrder(RequestOutOrder requestOutOrder)
        {
            Guard.Against.Null(requestOutOrder, nameof(requestOutOrder));
            Guard.Against.NullOrEmpty(requestOutOrder.RequestOutOrderRows, nameof(requestOutOrder.RequestOutOrderRows));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                ResponseResult responseResult = new ResponseResult();
                responseResult.Code = 200;
                try
                {
                    OrderSpecification orderSpec = new OrderSpecification(null, requestOutOrder.AlyNumber, null, null,
                        null, null, null, null, null, null, null,
                        null, null, null, null, null);
                    List<Order> orders = await this._orderRepository.ListAsync(orderSpec);

                    OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null,
                        requestOutOrder.AlyNumber, null, null, null, null, null);
                    List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);

                    OUSpecification ouSpec = new OUSpecification(null, null, requestOutOrder.BusinessEntity, null);
                    List<OU> ous = await this._ouRepository.ListAsync(ouSpec);
                    if (ous.Count == 0)
                        throw new Exception(string.Format("出库订单[{0}],关联业务实体编码[{1}]不存在!", requestOutOrder.AlyNumber,
                            requestOutOrder.BusinessEntity));
                    OU ou = ous[0];
                    WarehouseSpecification warehouseSpec =
                        new WarehouseSpecification(null, null, null, requestOutOrder.InventoryOrg);
                    List<Warehouse> warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                    if (warehouses.Count == 0)
                        throw new Exception(string.Format("出库订单[{0}],关联库存组织编码[{1}]不存在!", requestOutOrder.AlyNumber,
                            requestOutOrder.InventoryOrg));
                    Warehouse warehouse = warehouses[0];

                    EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(
                        Convert.ToInt32(requestOutOrder.ItemId), null,
                        null, null, null, null, null);
                    List<EBSProject> ebsProjects = await this._ebsProjectRepository.ListAsync(ebsProjectSpec);
                    if (ebsProjects.Count == 0)
                        throw new Exception(string.Format("出库订单[{0}],关联项目Id[{1}]不存在!", requestOutOrder.AlyNumber,
                            requestOutOrder.ItemId));
                    EmployeeSpecification employeeSpec =
                        new EmployeeSpecification(Convert.ToInt32(requestOutOrder.CreationBy), null, null, null);
                    List<Employee> employees = await this._employeeRepository.ListAsync(employeeSpec);
                    if (employees.Count == 0)
                        throw new Exception(string.Format("出库订单[{0}],关联经办人Id[{1}]不存在!", requestOutOrder.AlyNumber,
                            requestOutOrder.CreationBy));
                    OrganizationSpecification organizationSpec =
                        new OrganizationSpecification(null, requestOutOrder.AlyDepCode, null, null);
                    List<Organization> alyOrgs = await this._organizationRepository.ListAsync(organizationSpec);
                    if (alyOrgs.Count == 0)
                        throw new Exception(string.Format("出库订单[{0}],关联申请部门编码[{1}]不存在！", requestOutOrder.AlyNumber,
                            requestOutOrder.AlyDepCode));
                    Organization alyOrg = alyOrgs[0];
                    organizationSpec = new OrganizationSpecification(null, requestOutOrder.TransDepCode, null, null);
                    List<Organization> transOrgs = await this._organizationRepository.ListAsync(organizationSpec);
                    if (transOrgs.Count == 0)
                        throw new Exception(string.Format("出库订单[{0}],关联领料部门编码[{1}]不存在！", requestOutOrder.AlyNumber,
                            requestOutOrder.TransDepCode));


                    if (orders.Count > 0)
                    {
                        var srcOrder = orders[0];
                        if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                            throw new Exception(string.Format("出库订单[{0}]已经完成无法修改！", requestOutOrder.AlyNumber));
                        if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                            throw new Exception(string.Format("出库订单[{0}]已经关闭无法修改！", requestOutOrder.AlyNumber));
                        if (requestOutOrder.AlyStatusCode == "3")
                        {
                            if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.执行中))
                                throw new Exception(string.Format("出库订单[{0}]正在执行无法关闭！", requestOutOrder.AlyNumber));
                            List<Order> updOrders = new List<Order>();
                            List<OrderRow> updOrderRows = new List<OrderRow>();
                            int exeCount = orderRows.Count(r =>
                                r.OrderId == srcOrder.Id && r.Status == Convert.ToInt32(ORDER_STATUS.执行中));
                            if (exeCount > 0)
                                throw new Exception(string.Format("出库订单[{0}],正在执行中的订单行[{1}]个,无法关闭！",
                                    requestOutOrder.AlyNumber, exeCount));
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
                                LogDesc = "关闭出库订单[{0}]"+srcOrder.OrderNumber,
                                CreateTime = DateTime.Now
                            });
                            
                        }
                        else
                        {
                            List<OrderRow> addOrderRows = new List<OrderRow>();
                            List<OrderRow> updOrderRows = new List<OrderRow>();
                            requestOutOrder.RequestOutOrderRows.ForEach(async (eor) =>
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
                                        throw new Exception(string.Format("出库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                            requestOutOrder.AlyNumber, eor.LineNum, eor.MaterialId));
                                    MaterialDic materialDic = materialDics[0];
                                    EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(
                                        Convert.ToInt32(eor.TaskId), null,
                                        null,
                                        null, null, null, null);
                                    List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                                    if (ebsTasks.Count == 0)
                                        throw new Exception(string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                            requestOutOrder.AlyNumber, eor.LineNum, eor.TaskId));
                                    EBSTask ebsTask = ebsTasks[0];
                                    ReservoirAreaSpecification reservoirAreaSpec =
                                        new ReservoirAreaSpecification(null, eor.InventoryCode, null, null, null,
                                            null);
                                    List<ReservoirArea> areas =
                                        await this._areaRepository.ListAsync(reservoirAreaSpec);
                                    if (areas.Count == 0)
                                        throw new Exception(string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                            requestOutOrder.AlyNumber, eor.LineNum, eor.InventoryCode));
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
                                    if (existRow.Status!=Convert.ToInt32(ORDER_STATUS.完成)&&
                                        existRow.Status!=Convert.ToInt32(ORDER_STATUS.关闭))
                                    {
                                        //关闭
                                        if (eor.AlyStatusCode == "3")
                                        { 
                                            if(existRow.Status==Convert.ToInt32(ORDER_STATUS.执行中))
                                                throw new Exception(string.Format("修改出库订单[{0}],关闭订单行[{1}],关闭失败,订单行执行中",
                                                    requestOutOrder.AlyNumber,eor.LineNum));
                                        }
                                        if (Convert.ToInt32(eor.CancelQty) > (existRow.PreCount-existRow.Sorting))
                                        {
                                            throw new Exception(string.Format("修改出库订单[{0}],订单行[{1}],取消数量大于剩余数量,已出库[{2}],剩余[{3}]",
                                                requestOutOrder.AlyNumber,eor.LineNum,existRow.Sorting,existRow.PreCount-existRow.Sorting));
                                        }
                                        
                                        if (Convert.ToInt32(eor.ReqQty) < existRow.PreCount)
                                        {
                                            if (Convert.ToInt32(eor.ReqQty) < (existRow.PreCount - existRow.Sorting))
                                                throw new Exception(string.Format("修改出库订单[{0}],订单行[{1}],修改数量大于剩余数量,已出库[{2}],剩余[{3}]",
                                                    requestOutOrder.AlyNumber,eor.LineNum,existRow.Sorting,existRow.PreCount-existRow.Sorting));
                                        }
                                        existRow.CancelCount = Convert.ToInt32(eor.CancelQty);
                                        existRow.PreCount = Convert.ToInt32(eor.ReqQty);
                                        updOrderRows.Add(existRow);
                                    }
                                }
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
                                StringBuilder sb = new StringBuilder(string.Format("修改出库订单[{0}]\n",srcOrder.Id));
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
                                
                            });
                        }

                    }
                    else
                    {
                        Order addOrder = new Order
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
                        List<OrderRow> addOrderRows = new List<OrderRow>();
                        requestOutOrder.RequestOutOrderRows.ForEach(async (eor) =>
                        {
                            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                Convert.ToInt32(eor.MaterialId),
                                null, null, null, null);
                            List<MaterialDic> materialDics =
                                await this._materialDicRepository.ListAsync(materialDicSpec);
                            if (materialDics.Count == 0)
                                throw new Exception(string.Format("出库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                    requestOutOrder.AlyNumber, eor.LineNum, eor.MaterialId));
                            MaterialDic materialDic = materialDics[0];
                            EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(Convert.ToInt32(eor.TaskId),
                                null,
                                null,
                                null, null, null, null);
                            List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                            if (ebsTasks.Count == 0)
                                throw new Exception(string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                    requestOutOrder.AlyNumber, eor.LineNum, eor.TaskId));
                            EBSTask ebsTask = ebsTasks[0];
                            ReservoirAreaSpecification reservoirAreaSpec =
                                new ReservoirAreaSpecification(null, eor.InventoryCode, null, null, null, null);
                            List<ReservoirArea> areas = await this._areaRepository.ListAsync(reservoirAreaSpec);
                            if (areas.Count == 0)
                                throw new Exception(string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                    requestOutOrder.AlyNumber, eor.LineNum, eor.InventoryCode));
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
                        });
                        
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
}