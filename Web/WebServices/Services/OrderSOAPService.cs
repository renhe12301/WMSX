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
    public class OrderSOAPService : IOrderSOAPService
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
        private readonly IAsyncRepository<LogRecord> _logRepository;
        private readonly ILogRecordService _logRecordService;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;

        public OrderSOAPService()
        {
            this._ouRepository = EnginContext.Current.Resolve<IAsyncRepository<OU>>();
            this._orderRepository = EnginContext.Current.Resolve<IAsyncRepository<Order>>();
            this._warehouseRepository = EnginContext.Current.Resolve<IAsyncRepository<Warehouse>>();
            this._supplierRepository = EnginContext.Current.Resolve<IAsyncRepository<Supplier>>();
            this._supplierSiteRepository = EnginContext.Current.Resolve<IAsyncRepository<SupplierSite>>();
            this._materialDicRepository = EnginContext.Current.Resolve<IAsyncRepository<MaterialDic>>();
            this._ebsProjectRepository = EnginContext.Current.Resolve<IAsyncRepository<EBSProject>>();
            this._ebsTaskRepository = EnginContext.Current.Resolve<IAsyncRepository<EBSTask>>();
            this._employeeRepository = EnginContext.Current.Resolve<IAsyncRepository<Employee>>();
            this._organizationRepository = EnginContext.Current.Resolve<IAsyncRepository<Organization>>();
            this._areaRepository = EnginContext.Current.Resolve<IAsyncRepository<ReservoirArea>>();
            this._logRecordService = EnginContext.Current.Resolve<ILogRecordService>();
            this._logRepository = EnginContext.Current.Resolve<IAsyncRepository<LogRecord>>();
            ;
            this._orderRowRepository = EnginContext.Current.Resolve<IAsyncRepository<OrderRow>>();
            this._subOrderRowRepository = EnginContext.Current.Resolve<IAsyncRepository<SubOrderRow>>();
        }

        public async Task<ResponseResult> CreateRKJSOrder(string RequestRKJSOrderJson)
        {
            //Guard.Against.Null(RequestRKJSOrders, nameof(RequestRKJSOrders));
            ResponseResult responseResult = new ResponseResult();
            responseResult.Code = 200;
            
            List<RequestRKJSOrder> RequestRKJSOrders = null;
            try
            {
                RequestRKJSOrders =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<RequestRKJSOrder>>(RequestRKJSOrderJson);
            }
            catch (Exception ex)
            {
                responseResult.Data = ex.Message;
                responseResult.Code = 500;
                return responseResult;
            }
            
                using (ModuleLock.GetAsyncLock().LockAsync())
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        try
                        {

                            foreach (var RequestRKJSOrder in RequestRKJSOrders)
                            {
                                OrderSpecification orderSpec = new OrderSpecification(null,
                                    RequestRKJSOrder.DocumentNumber,
                                    null, null, null, null, null, null, null, null,
                                    null, null, null, null, null,
                                    null, null, null, null, null, null, null);
                                List<Order> orders = this._orderRepository.List(orderSpec);

                                OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null, null, null,
                                    RequestRKJSOrder.DocumentNumber, null, null, null, null, null);
                                List<OrderRow> orderRows = this._orderRowRepository.List(orderRowSpec);

                                #region 字段合法性校验

                                OUSpecification ouSpec = new OUSpecification(null, null, RequestRKJSOrder.OuCode, null);
                                List<OU> ous = this._ouRepository.List(ouSpec);
                                if (ous.Count == 0)
                                {
                                    string err = string.Format("入库订单[{0}],关联业务实体编码[{1}]不存在!",
                                        RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.OuCode);
                                    throw new Exception(err);
                                }

                                OU ou = ous[0];
                                WarehouseSpecification warehouseSpec =
                                    new WarehouseSpecification(null, null, null, RequestRKJSOrder.OrganizationCode);
                                List<Warehouse> warehouses = this._warehouseRepository.List(warehouseSpec);
                                if (warehouses.Count == 0)
                                {
                                    string err = string.Format("入库订单[{0}],关联库存组织编码[{1}]不存在!",
                                        RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.OrganizationCode);
                                    throw new Exception(err);
                                }

                                Warehouse warehouse = warehouses[0];

                                SupplierSpecification supplierSpec =
                                    new SupplierSpecification(Convert.ToInt32(RequestRKJSOrder.VendorId), null);
                                List<Supplier> suppliers = this._supplierRepository.List(supplierSpec);
                                if (suppliers.Count == 0)
                                {
                                    string err = string.Format("入库订单[{0}],关联供应商头Id[{1}]不存在!",
                                        RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.VendorId);
                                    throw new Exception(err);
                                }

                                Supplier supplier = suppliers[0];

                                SupplierSiteSpecification supplierSiteSpec = new SupplierSiteSpecification(
                                    Convert.ToInt32(RequestRKJSOrder.VendorSiteId),
                                    null, null, null);
                                List<SupplierSite> supplierSites = this._supplierSiteRepository.List(supplierSiteSpec);
                                if (supplierSites.Count == 0)
                                {
                                    string err = string.Format("入库订单[{0}],关联供应商地址Id[{1}]不存在!",
                                        RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.VendorSiteId);
                                    throw new Exception(err);
                                }

                                SupplierSite supplierSite = supplierSites[0];

                                // EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(
                                //     Convert.ToInt32(RequestRKJSOrder.ItemId), null,
                                //     null, null, null, null, null);
                                // List<EBSProject> ebsProjects = this._ebsProjectRepository.List(ebsProjectSpec);
                                // if (ebsProjects.Count == 0)
                                // {
                                //     string err = string.Format("入库订单[{0}],关联项目Id[{0}]不存在!",
                                //         RequestRKJSOrder.DocumentNumber,
                                //         RequestRKJSOrder.ItemId);
                                //     throw new Exception(err);
                                // }

                                EmployeeSpecification employeeSpec =
                                    new EmployeeSpecification(Convert.ToInt32(RequestRKJSOrder.ManagerId), null, null,
                                        null);
                                List<Employee> employees = this._employeeRepository.List(employeeSpec);
                                if (employees.Count == 0)
                                {
                                    string err = string.Format("入库订单[{0}],关联经办人Id[{1}]不存在!",
                                        RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.ManagerId);
                                    throw new Exception(err);
                                }

                                #endregion

                                if (orders.Count > 0)
                                {
                                    var srcOrder = orders[0];
                                    if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                                    {
                                        string err = string.Format("入库订单[{0}]已经完成无法修改！",
                                            RequestRKJSOrder.DocumentNumber);
                                        throw new Exception(err);
                                    }

                                    if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                    {
                                        string err = string.Format("入库订单[{0}]已经关闭无法修改！",
                                            RequestRKJSOrder.DocumentNumber);
                                        throw new Exception(err);
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
                                                this._materialDicRepository.List(materialDicSpec);
                                            if (materialDics.Count == 0)
                                            {
                                                string err = string.Format("入库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                                    RequestRKJSOrder.DocumentNumber, eor.LineNumber, eor.MaterialId);
                                                throw new Exception(err);
                                            }

                                            MaterialDic materialDic = materialDics[0];
                                            // EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(
                                            //     Convert.ToInt32(eor.TaskId),
                                            //     null, null,
                                            //     null, null, null, null);
                                            // List<EBSTask> ebsTasks = this._ebsTaskRepository.List(ebsTaskSpec);
                                            // if (ebsTasks.Count == 0)
                                            // {
                                            //     string err = string.Format("入库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                            //         RequestRKJSOrder.DocumentNumber, eor.LineNumber, eor.TaskId);
                                            //     throw new Exception(err);
                                            // }
                                            //
                                            // EBSTask ebsTask = ebsTasks[0];
                                            OrderRow addOrderRow = new OrderRow();
                                            addOrderRow.OrderId = srcOrder.Id;
                                            addOrderRow.SourceId = Convert.ToInt32(eor.LineId);
                                            addOrderRow.RowNumber = eor.LineNumber;
                                            addOrderRow.MaterialDicId = materialDic.Id;
                                            addOrderRow.PreCount = Convert.ToInt32(eor.ProcessingQuantity);
                                            addOrderRow.Price = Convert.ToInt32(eor.Price);
                                            addOrderRow.Amount = Convert.ToInt32(eor.Amount);
                                            //addOrderRow.EBSTaskId = ebsTask.Id;
                                            addOrderRow.Memo = eor.Remark;
                                            addOrderRows.Add(addOrderRow);
                                        }

                                    }

                                    if (addOrderRows.Count > 0)
                                    {
                                        this._orderRowRepository.Add(addOrderRows);
                                        StringBuilder sb =
                                            new StringBuilder(string.Format("修改入库订单[{0}]\n", srcOrder.Id));

                                        sb.Append(string.Format("新增入库订单行[{0}]\n",
                                            string.Join(',', addOrderRows.ConvertAll(r => r.Id))));

                                        this._logRepository.Add(new LogRecord
                                        {
                                            LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                            LogDesc = sb.ToString(),
                                            CreateTime = DateTime.Now
                                        });
                                    }
                                }
                                else
                                {
                                    // 入库接收单，接收退料
                                    if (RequestRKJSOrder.DocumentType == "RKTL")
                                    {
                                        orderRows.ForEach(or =>
                                        {
                                            int reId = or.RelatedId.GetValueOrDefault();
                                            SubOrderRowSpecification subOrderRowSpecification =
                                                new SubOrderRowSpecification(null,
                                                    null, null, reId, null, null, null, null, null, null,
                                                    null, null, null, null, null, null, null);
                                            List<SubOrderRow> subOrderRows =
                                                this._subOrderRowRepository.List(subOrderRowSpecification);
                                            if (subOrderRows.Count > 0)
                                                throw new Exception("前置订单行[{0}]在WMS系统中已经拆分无法进行撤销操作!");
                                        });
                                    }

                                    Order addOrder = new Order();

                                    addOrder.SourceId = Convert.ToInt32(RequestRKJSOrder.HeaderId);
                                    addOrder.OrderNumber = RequestRKJSOrder.DocumentNumber;
                                    addOrder.EmployeeId = Convert.ToInt32(RequestRKJSOrder.ManagerId);
                                    addOrder.OUId = ou.Id;
                                    addOrder.OrderTypeId = GetOrderType(RequestRKJSOrder.DocumentType);
                                    addOrder.WarehouseId = warehouse.Id;
                                    addOrder.SupplierId = supplier.Id;
                                    addOrder.SupplierSiteId = supplierSite.Id;
                                    addOrder.BusinessTypeCode = RequestRKJSOrder.BusinessType;
                                    addOrder.Currency = RequestRKJSOrder.Currency;
                                    addOrder.TotalAmount = Convert.ToDouble(RequestRKJSOrder.TotalAmount);
                                    addOrder.ApplyTime = DateTime.Parse(RequestRKJSOrder.ExitEntryDate);
                                    addOrder.CreateTime = DateTime.Parse(RequestRKJSOrder.CreationDate);
                                    //addOrder.EBSProjectId = Convert.ToInt32(RequestRKJSOrder.ItemId),
                                    addOrder.Memo = RequestRKJSOrder.Remark;

                                    List<OrderRow> addOrderRows = new List<OrderRow>();
                                    foreach (var eor in RequestRKJSOrder.RequestRKJSRows)
                                    {
                                        MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                            Convert.ToInt32(eor.MaterialId),
                                            null, null, null, null);
                                        List<MaterialDic> materialDics =
                                            this._materialDicRepository.List(materialDicSpec);
                                        if (materialDics.Count == 0)
                                        {
                                            string err = string.Format("入库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                                RequestRKJSOrder.DocumentNumber, eor.LineNumber, eor.MaterialId);
                                            throw new Exception(err);
                                        }

                                        MaterialDic materialDic = materialDics[0];
                                        // EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(
                                        //     Convert.ToInt32(eor.TaskId),
                                        //     null, null,
                                        //     null, null, null, null);
                                        // List<EBSTask> ebsTasks = this._ebsTaskRepository.List(ebsTaskSpec);
                                        // if (ebsTasks.Count == 0)
                                        // {
                                        //     string err = string.Format("入库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                        //         RequestRKJSOrder.DocumentNumber, eor.LineNumber, eor.TaskId);
                                        //     throw new Exception(err);
                                        // }
                                        //
                                        // EBSTask ebsTask = ebsTasks[0];
                                        OrderRow addOrderRow = new OrderRow();

                                        addOrderRow.SourceId = Convert.ToInt32(eor.LineId);
                                        addOrderRow.RowNumber = eor.LineNumber;
                                        addOrderRow.MaterialDicId = materialDic.Id;
                                        addOrderRow.PreCount = Convert.ToInt32(eor.ProcessingQuantity);
                                        addOrderRow.Price = Convert.ToDouble(eor.Price);
                                        addOrderRow.Amount = Convert.ToDouble(eor.Amount);
                                        //EBSTaskId = ebsTask.Id,
                                        addOrderRow.Memo = eor.Remark;

                                        
                                        if (!string.IsNullOrEmpty(eor.RelatedId))
                                        {
                                            addOrderRow.RelatedId = Convert.ToInt32(eor.RelatedId);
                                        }

                                        addOrderRows.Add(addOrderRow);
                                    }

                                    Order saveOrder = this._orderRepository.Add(addOrder);
                                    addOrderRows.ForEach(om => om.OrderId = saveOrder.Id);
                                    this._orderRowRepository.Add(addOrderRows);
                                    this._logRepository.Add(new LogRecord
                                    {
                                        LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                        LogDesc = string.Format("新增入库订单[{0}]\n新增入库订单行[{1}]", addOrder.Id,
                                            string.Join(',', addOrderRows.ConvertAll(r => r.Id))),
                                        CreateTime = DateTime.Now
                                    });
                                }

                            }

                            scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            responseResult.Code = 500;
                            responseResult.Data = ex.Message;
                            this._logRepository.Add(new LogRecord
                            {
                                LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                                LogDesc = ex.Message,
                                CreateTime = DateTime.Now
                            });
                            return responseResult;
                        }
                    }

                    responseResult.Data = "Success!";
                    return responseResult;
                }
                
     
        }
    

        public  async Task<ResponseResult> CreateCKLLOrder(string RequestCKLLOrderJson)
        {
            //Guard.Against.Null(RequestCKLLOrders, nameof(RequestCKLLOrders));
            
            ResponseResult responseResult = new ResponseResult();
            responseResult.Code = 200;
            
            List<RequestCKLLOrder> RequestCKLLOrders = null;
            try
            {
                RequestCKLLOrders =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<RequestCKLLOrder>>(RequestCKLLOrderJson);
            }
            catch (Exception ex)
            {
                responseResult.Data = ex.Message;
                responseResult.Code = 500;
                return responseResult;
            }

            
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
               
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        foreach (var RequestCKLLOrder in RequestCKLLOrders)
                        {
                            OrderSpecification orderSpec = new OrderSpecification(null, RequestCKLLOrder.AlyNumber,
                                null,null,
                                null, null, null, null, null, null, null, null, null, null, null,
                                null, null, null, null, null, null, null);
                            List<Order> orders = this._orderRepository.List(orderSpec);

                            OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null, null, null,
                                RequestCKLLOrder.AlyNumber, null, null, null, null, null);
                            List<OrderRow> orderRows = this._orderRowRepository.List(orderRowSpec);

                            #region 字段合法性校验

                            OUSpecification ouSpec =
                                new OUSpecification(null, null, RequestCKLLOrder.BusinessEntity, null);
                            List<OU> ous = this._ouRepository.List(ouSpec);
                            if (ous.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联业务实体编码[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.BusinessEntity);
                                throw new Exception(err);
                            }

                            OU ou = ous[0];
                            WarehouseSpecification warehouseSpec =
                                new WarehouseSpecification(null, null, null, RequestCKLLOrder.InventoryOrg);
                            List<Warehouse> warehouses = this._warehouseRepository.List(warehouseSpec);
                            if (warehouses.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联库存组织编码[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.InventoryOrg);
                                throw new Exception(err);
                            }

                            Warehouse warehouse = warehouses[0];

                            // EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(
                            //     Convert.ToInt32(RequestCKLLOrder.ItemId), null,
                            //     null, null, null, null, null);
                            // List<EBSProject> ebsProjects = this._ebsProjectRepository.List(ebsProjectSpec);
                            // if (ebsProjects.Count == 0)
                            // {
                            //     string err = string.Format("出库订单[{0}],关联项目Id[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                            //         RequestCKLLOrder.ItemId);
                            //     throw new Exception(err);
                            // }

                            EmployeeSpecification employeeSpec =
                                new EmployeeSpecification(Convert.ToInt32(RequestCKLLOrder.CreationBy), null, null,
                                    null);
                            List<Employee> employees = this._employeeRepository.List(employeeSpec);
                            if (employees.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联经办人Id[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.CreationBy);
                                throw new Exception(err);
                            }

                            OrganizationSpecification organizationSpec =
                                new OrganizationSpecification(null, RequestCKLLOrder.AlyDepCode, null, null);
                            List<Organization> alyOrgs = this._organizationRepository.List(organizationSpec);
                            if (alyOrgs.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联申请部门编码[{1}]不存在！", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.AlyDepCode);
                                throw new Exception(err);
                            }

                            Organization alyOrg = alyOrgs[0];
                            organizationSpec =
                                new OrganizationSpecification(null, RequestCKLLOrder.TransDepCode, null, null);
                            List<Organization> transOrgs = this._organizationRepository.List(organizationSpec);
                            if (transOrgs.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联领料部门编码[{1}]不存在！", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.TransDepCode);
                                throw new Exception(err);
                            }

                            #endregion

                            if (orders.Count > 0)
                            {
                                var srcOrder = orders[0];
                                if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                                {
                                    string err = string.Format("出库订单[{0}]已经完成无法修改！", RequestCKLLOrder.AlyNumber);
                                    throw new Exception(err);

                                }

                                if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                {
                                    string err = string.Format("出库订单[{0}]已经关闭无法修改！", RequestCKLLOrder.AlyNumber);
                                    throw new Exception(err);

                                }

                                if (RequestCKLLOrder.AlyStatusCode == "3")
                                {
                                    if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.执行中) ||
                                        srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成) ||
                                        srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                    {
                                        string err = string.Format("出库订单[{0}][{1}],无法撤销！", RequestCKLLOrder.AlyNumber,
                                            Enum.GetName(typeof(ORDER_STATUS), srcOrder.Status));
                                        throw new Exception(err);
                                    }

                                    orderRows.ForEach(or =>
                                    {
                                        int reId = or.RelatedId.GetValueOrDefault();
                                        SubOrderRowSpecification subOrderRowSpecification =
                                            new SubOrderRowSpecification(null,
                                                null, null, reId,null, null, null, null, null, null,
                                                null, null, null, null, null, null, null);
                                        List<SubOrderRow> subOrderRows =
                                            this._subOrderRowRepository.List(subOrderRowSpecification);
                                        if (subOrderRows.Count > 0)
                                            throw new Exception("前置订单行[{0}]在WMS系统中已经拆分无法进行撤销操作!");
                                    });

                                    this._orderRepository.Update(srcOrder);
                                    this._orderRowRepository.Update(orderRows);

                                    this._logRepository.Add(new LogRecord
                                    {
                                        LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                        LogDesc = "撤销出库订单[{0}]" + srcOrder.OrderNumber,
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
                                            List<MaterialDic> materialDics = this._materialDicRepository.List(materialDicSpec);
                                            if (materialDics.Count == 0)
                                            {
                                                string err = string.Format("出库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                                    RequestCKLLOrder.AlyNumber, eor.LineNum, eor.MaterialId);
                                                throw new Exception(err);
                                            }

                                            MaterialDic materialDic = materialDics[0];
                                            // EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(
                                            //     Convert.ToInt32(eor.TaskId), null,
                                            //     null,
                                            //     null, null, null, null);
                                            // List<EBSTask> ebsTasks =
                                            //     await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                                            // if (ebsTasks.Count == 0)
                                            // {
                                            //     string err = string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                            //         RequestCKLLOrder.AlyNumber, eor.LineNum, eor.TaskId);
                                            //     throw new Exception(err);
                                            // }

                                            ReservoirAreaSpecification reservoirAreaSpec =
                                                new ReservoirAreaSpecification(null, eor.InventoryCode, null, null,
                                                    null,
                                                    null);
                                            List<ReservoirArea> areas =
                                                 this._areaRepository.List(reservoirAreaSpec);
                                            if (areas.Count == 0)
                                            {
                                                string err = string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                                    RequestCKLLOrder.AlyNumber, eor.LineNum, eor.InventoryCode);
                                                throw new Exception(err);
                                            }

                                            ReservoirArea area = areas[0];
                                            OrderRow addOrderRow = new OrderRow();
                                            addOrderRow.SourceId = Convert.ToInt32(eor.LineId);
                                            addOrderRow.RowNumber = eor.LineNum;
                                            addOrderRow.MaterialDicId = materialDic.Id;
                                            addOrderRow.UseFor = eor.UseFor;
                                            addOrderRow.PreCount = Convert.ToInt32(eor.ReqQty);
                                            addOrderRow.CancelCount = Convert.ToInt32(eor.CancelQty);
                                            addOrderRow.ReservoirAreaId = area.Id;
                                            addOrderRow.EBSTaskId = Convert.ToInt32(eor.TaskId);
                                            addOrderRow.Memo = eor.Remark;
                                            addOrderRows.Add(addOrderRow);
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(eor.CancelQty) > 0)
                                            {
                                                int reId = existRow.RelatedId.GetValueOrDefault();
                                                SubOrderRowSpecification subOrderRowSpecification =
                                                    new SubOrderRowSpecification(null,
                                                        null, null, reId,null, null, null, null, null, null,
                                                        null, null, null, null, null, null, null);
                                                List<SubOrderRow> subOrderRows =
                                                    this._subOrderRowRepository.List(subOrderRowSpecification);
                                                if (subOrderRows.Count > 0)
                                                    throw new Exception("前置订单行[{0}]在WMS系统中已经拆分无法取消数量操作!");
                                            }

                                            existRow.CancelCount = Convert.ToInt32(eor.CancelQty);
                                            existRow.PreCount = Convert.ToInt32(eor.ReqQty);
                                            updOrderRows.Add(existRow);
                                        }

                                        this._orderRowRepository.Add(addOrderRows);
                                        this._orderRowRepository.Update(updOrderRows);
                                        StringBuilder sb =
                                            new StringBuilder(string.Format("修改出库订单[{0}]\n", srcOrder.Id));
                                        if (addOrderRows.Count > 0)
                                            sb.Append(string.Format("新增出库订单行[{0}]\n",
                                                string.Join(',', addOrderRows.ConvertAll(r => r.Id))));
                                        if (updOrderRows.Count > 0)
                                            sb.Append(string.Format("修改出库订单行[{0}]\n",
                                                string.Join(',', updOrderRows.ConvertAll(r => r.Id))));

                                        this._logRepository.Add(new LogRecord
                                        {
                                            LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                            LogDesc = sb.ToString(),
                                            CreateTime = DateTime.Now
                                        });
                                    }
                                }

                            }
                            else
                            {
                                Order addOrder = new Order();
                                addOrder.SourceId = Convert.ToInt32(RequestCKLLOrder.HeaderId);
                                addOrder.OrderNumber = RequestCKLLOrder.AlyNumber;
                                addOrder.EmployeeId = Convert.ToInt32(RequestCKLLOrder.CreationBy);
                                addOrder.ApplyUserCode = RequestCKLLOrder.AlyDepCode;
                                addOrder.ApproveUserCode = RequestCKLLOrder.TransDepCode;
                                addOrder.OUId = ou.Id;
                                addOrder.OrderTypeId = GetOrderType(RequestCKLLOrder.DocumentType);
                                addOrder.WarehouseId = warehouse.Id;
                                addOrder.CallingParty = RequestCKLLOrder.AplSourceCode;
                                addOrder.BusinessTypeCode = RequestCKLLOrder.BusinessTypeCode;
                                addOrder.CreateTime = DateTime.Parse(RequestCKLLOrder.CreationDate);
                                //addOrder.EBSProjectId = Convert.ToInt32(RequestCKLLOrder.ItemId);
                                addOrder.Memo = RequestCKLLOrder.Remark;
                                List<OrderRow> addOrderRows = new List<OrderRow>();
                                foreach (var eor in RequestCKLLOrder.RequestCKLLRows)
                                {
                                    MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                        Convert.ToInt32(eor.MaterialId),
                                        null, null, null, null);
                                    List<MaterialDic> materialDics = this._materialDicRepository.List(materialDicSpec);
                                    if (materialDics.Count == 0)
                                    {
                                        string err = string.Format("出库订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                            RequestCKLLOrder.AlyNumber, eor.LineNum, eor.MaterialId);
                                        throw new Exception(err);
                                    }

                                    MaterialDic materialDic = materialDics[0];
                                    // EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(
                                    //     Convert.ToInt32(eor.TaskId),
                                    //     null,
                                    //     null,
                                    //     null, null, null, null);
                                    // List<EBSTask> ebsTasks = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                                    // if (ebsTasks.Count == 0)
                                    // {
                                    //     string err = string.Format("出库订单[{0}],订单行[{1}],关联任务Id[{2}]不存在！",
                                    //         RequestCKLLOrder.AlyNumber, eor.LineNum, eor.TaskId);
                                    //     throw new Exception(err);
                                    // }

                                    ReservoirAreaSpecification reservoirAreaSpec =
                                        new ReservoirAreaSpecification(null, eor.InventoryCode, null, null, null, null);
                                    List<ReservoirArea> areas = this._areaRepository.List(reservoirAreaSpec);
                                    if (areas.Count == 0)
                                    {
                                        string err = string.Format("出库订单[{0}],订单行[{1}],关联分区Code[{2}]不存在！",
                                            RequestCKLLOrder.AlyNumber, eor.LineNum, eor.InventoryCode);
                                        throw new Exception(err);
                                    }

                                    ReservoirArea area = areas[0];
                                    OrderRow addOrderRow = new OrderRow();
                                    addOrderRow.SourceId = Convert.ToInt32(eor.LineId);
                                    addOrderRow.RowNumber = eor.LineNum;
                                    addOrderRow.MaterialDicId = materialDic.Id;
                                    addOrderRow.UseFor = eor.UseFor;
                                    addOrderRow.PreCount = Convert.ToInt32(eor.ReqQty);
                                    addOrderRow.CancelCount = Convert.ToInt32(eor.CancelQty);
                                    addOrderRow.ReservoirAreaId = area.Id;
                                    //EBSTaskId = Convert.ToInt32(eor.TaskId),
                                    addOrderRow.Memo = eor.Remark;
                                    addOrderRows.Add(addOrderRow);
                                }

                                Order saveOrder = this._orderRepository.Add(addOrder);
                                addOrderRows.ForEach(om => om.OrderId = saveOrder.Id);
                                this._orderRowRepository.Add(addOrderRows);
                                this._logRepository.Add(new LogRecord
                                {
                                    LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                    LogDesc = string.Format("新增出库订单[{0}]\n新增出库订单行[{1}]", addOrder.Id,
                                        string.Join(',', addOrderRows.ConvertAll(r => r.Id))),
                                    CreateTime = DateTime.Now
                                });

                            }

                        }
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        responseResult.Code = 500;
                        responseResult.Data = ex.Message;
                        this._logRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                            LogDesc = ex.Message,
                            CreateTime = DateTime.Now
                        });
                        return responseResult;
                    }
                }

                responseResult.Data = "Success!";
                return responseResult;
            }

        }

        public string Hello(string name)
        {
            return "服务器响应：" + name;
        }

        int GetOrderType(string documentType)
        {
            int orderType = 0;
            if (documentType.Equals("RECEIPT"))
                orderType = Convert.ToInt32(ORDER_TYPE.入库接收);
            else if(documentType.Equals("PICK"))
                orderType = Convert.ToInt32(ORDER_TYPE.出库领料);
            return orderType;
        }

    }
}