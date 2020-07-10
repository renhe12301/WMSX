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
using ApplicationCore.Entities.StockManager;
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
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;

        public OrderSOAPService(IAsyncRepository<Order> orderRepository,
            IAsyncRepository<OU> ouRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<Supplier> supplierRepository,
            IAsyncRepository<SupplierSite> supplierSiteRepository,
            IAsyncRepository<MaterialDic> materialDicRepository,
            IAsyncRepository<Employee> employeeRepository,
            IAsyncRepository<Organization> organizationRepository,
            IAsyncRepository<ReservoirArea> areaRepository,
            IAsyncRepository<LogRecord> logRepository,
            IAsyncRepository<OrderRow> orderRowRepository,
            IAsyncRepository<SubOrderRow> subOrderRowRepository
            
            )
        {
            this._ouRepository = ouRepository;
            this._orderRepository = orderRepository;
            this._warehouseRepository = warehouseRepository;
            this._supplierRepository = supplierRepository;
            this._supplierSiteRepository = supplierSiteRepository;
            this._materialDicRepository = materialDicRepository;
            this._employeeRepository = employeeRepository;
            this._organizationRepository = organizationRepository;
            this._areaRepository = areaRepository;
            this._logRepository = logRepository;
            this._orderRowRepository = orderRowRepository;
            this._subOrderRowRepository = subOrderRowRepository;
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

            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        if (RequestRKJSOrders.Count == 0)
                            throw new Exception("入库订单不能为空!");
                        foreach (var RequestRKJSOrder in RequestRKJSOrders)
                        {
                            OrderSpecification orderSpec = new OrderSpecification(null,
                                RequestRKJSOrder.DocumentNumber,
                                null, null, null, null, null, null, null, null,
                                null, null, null, null, null,
                                null, null, null, null, null, null, null);
                            List<Order> orders = this._orderRepository.List(orderSpec);

                            OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null, null, null,
                                RequestRKJSOrder.DocumentNumber,null,null, null, null, null, null, null,null,
                                null, null, null, null, null, null);
                            List<OrderRow> orderRows = this._orderRowRepository.List(orderRowSpec);

                            #region 字段合法性校验

                            if (string.IsNullOrEmpty(RequestRKJSOrder.OuCode))
                                throw new Exception("业务实体Id不能为空!");

                            OUSpecification ouSpec = new OUSpecification(Convert.ToInt32(RequestRKJSOrder.OuCode), null, null, null);
                            List<OU> ous = this._ouRepository.List(ouSpec);
                            if (ous.Count == 0)
                            {
                                string err = string.Format("入库订单[{0}],关联业务实体[{1}]不存在!",
                                    RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.OuCode);
                                throw new Exception(err);
                            }

                            OU ou = ous[0];

                            if (string.IsNullOrEmpty(RequestRKJSOrder.OrganizationCode))
                                throw new Exception("库存组织Id不能为空!");
                            WarehouseSpecification warehouseSpec =
                                new WarehouseSpecification(Convert.ToInt32(RequestRKJSOrder.OrganizationCode), null, null, null);
                            List<Warehouse> warehouses = this._warehouseRepository.List(warehouseSpec);
                            if (warehouses.Count == 0)
                            {
                                string err = string.Format("入库订单[{0}],关联库存组织[{1}]不存在!",
                                    RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.OrganizationCode);
                                throw new Exception(err);
                            }

                            Warehouse warehouse = warehouses[0];

                            if (string.IsNullOrEmpty(RequestRKJSOrder.VendorId))
                                throw new Exception("供应商头Id不能为空!");
                            SupplierSpecification supplierSpec =
                                new SupplierSpecification(Convert.ToInt32(RequestRKJSOrder.VendorId), null);
                            List<Supplier> suppliers = this._supplierRepository.List(supplierSpec);
                            if (suppliers.Count == 0)
                            {
                                string err = string.Format("入库订单[{0}],关联供应商头[{1}]不存在!",
                                    RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.VendorId);
                                throw new Exception(err);
                            }

                            Supplier supplier = suppliers[0];

                            if (string.IsNullOrEmpty(RequestRKJSOrder.VendorSiteId))
                                throw new Exception("供应商地址Id不能为空!");
                            SupplierSiteSpecification supplierSiteSpec = new SupplierSiteSpecification(
                                Convert.ToInt32(RequestRKJSOrder.VendorSiteId),
                                null, null, null);
                            List<SupplierSite> supplierSites = this._supplierSiteRepository.List(supplierSiteSpec);
                            if (supplierSites.Count == 0)
                            {
                                string err = string.Format("入库订单[{0}],关联供应商地址[{1}]不存在!",
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

                            if (string.IsNullOrEmpty(RequestRKJSOrder.ManagerId))
                                throw new Exception("经办人Id不能为空!");
                            EmployeeSpecification employeeSpec =
                                new EmployeeSpecification(Convert.ToInt32(RequestRKJSOrder.ManagerId), null, null,
                                    null);
                            List<Employee> employees = this._employeeRepository.List(employeeSpec);
                            if (employees.Count == 0)
                            {
                                string err = string.Format("入库订单[{0}],关联经办人[{1}]不存在!",
                                    RequestRKJSOrder.DocumentNumber, RequestRKJSOrder.ManagerId);
                                throw new Exception(err);
                            }

                            #endregion

                            if (orders.Count > 0)
                            {
                                var srcOrder = orders[0];
                                // if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                                // {
                                //     string err = string.Format("入库订单[{0}]已经完成无法修改！",
                                //         RequestRKJSOrder.DocumentNumber);
                                //     throw new Exception(err);
                                // }
                                //
                                // if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                // {
                                //     string err = string.Format("入库订单[{0}]已经关闭无法修改！",
                                //         RequestRKJSOrder.DocumentNumber);
                                //     throw new Exception(err);
                                // }

                                List<OrderRow> addOrderRows = new List<OrderRow>();
                                foreach (var eor in RequestRKJSOrder.RequestRKJSRows)
                                {
                                    var existRow = orderRows.Find(r => r.RowNumber == eor.LineNumber);
                                    if (existRow == null)
                                    {
                                        if (string.IsNullOrEmpty(eor.MaterialId))
                                            throw new Exception("物料Id不能为空!");
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
                                        addOrderRow.ExpenditureType = eor.ExpenditrueType;
                                        if (!string.IsNullOrEmpty(eor.TaskId))
                                            addOrderRow.EBSTaskId = Convert.ToInt32(eor.TaskId);
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
                                                null, null, reId, null, null, null, null,null,null, null, null,
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
                                addOrder.SourceOrderType = RequestRKJSOrder.DocumentType;
                                if(!string.IsNullOrEmpty(RequestRKJSOrder.ItemId))
                                   addOrder.EBSProjectId = Convert.ToInt32(RequestRKJSOrder.ItemId);
                                addOrder.Memo = RequestRKJSOrder.Remark;

                                List<OrderRow> addOrderRows = new List<OrderRow>();
                                foreach (var eor in RequestRKJSOrder.RequestRKJSRows)
                                {
                                    if (string.IsNullOrEmpty(eor.MaterialId))
                                        throw new Exception("物料Id不能为空!");
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
                                    addOrderRow.PreCount = Convert.ToDouble(eor.ProcessingQuantity);
                                    addOrderRow.Price = Convert.ToDouble(eor.Price);
                                    addOrderRow.Amount = Convert.ToDouble(eor.Amount);
                                    addOrderRow.ExpenditureType = eor.ExpenditrueType;
                                    if (!string.IsNullOrEmpty(eor.TaskId))
                                        addOrderRow.EBSTaskId = Convert.ToInt32(eor.TaskId);
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

            
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        if (RequestCKLLOrders.Count == 0)
                            throw new Exception("出库订单不能为空!");
                        foreach (var RequestCKLLOrder in RequestCKLLOrders)
                        {
                            OrderSpecification orderSpec = new OrderSpecification(null, RequestCKLLOrder.AlyNumber,
                                null,null,
                                null, null, null, null, null, null, null, null, null, null, null,
                                null, null, null, null, null, null, null);
                            List<Order> orders = this._orderRepository.List(orderSpec);

                            OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null, null, null,
                                RequestCKLLOrder.AlyNumber,null,null, null, null, null, null, null,null,
                                null,null,null,null,null,null);
                            List<OrderRow> orderRows = this._orderRowRepository.List(orderRowSpec);

                            #region 字段合法性校验
                            if (string.IsNullOrEmpty(RequestCKLLOrder.BusinessEntity))
                                throw new Exception("业务实体Id不能为空!");
                            OUSpecification ouSpec =
                                new OUSpecification(Convert.ToInt32(RequestCKLLOrder.BusinessEntity), null, null, null);
                            List<OU> ous = this._ouRepository.List(ouSpec);
                            if (ous.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联业务实体[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.BusinessEntity);
                                throw new Exception(err);
                            }

                            OU ou = ous[0];

                            if (string.IsNullOrEmpty(RequestCKLLOrder.InventoryOrg))
                                throw new Exception("库存组织Id不能为空!");
                            WarehouseSpecification warehouseSpec =
                                new WarehouseSpecification(Convert.ToInt32(RequestCKLLOrder.InventoryOrg), null, null, null);
                            List<Warehouse> warehouses = this._warehouseRepository.List(warehouseSpec);
                            if (warehouses.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联库存组织[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.InventoryOrg);
                                throw new Exception(err);
                            }

                            Warehouse warehouse = warehouses[0];

                            if (string.IsNullOrEmpty(RequestCKLLOrder.CreationBy))
                                throw new Exception("制单人Id不能为空!");
                            EmployeeSpecification employeeSpecification = new EmployeeSpecification(Convert.ToInt32(RequestCKLLOrder.CreationBy), null, null, null);
                            List<Employee> employees = this._employeeRepository.List(employeeSpecification);
                            if (employees.Count == 0)
                                throw new Exception(string.Format("出库订单[{0}],关联制单人Id[{1}]不存在!",RequestCKLLOrder.AlyNumber, RequestCKLLOrder.CreationBy));

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

                            // EmployeeSpecification employeeSpec =
                            //     new EmployeeSpecification(Convert.ToInt32(RequestCKLLOrder.CreationBy), null, null,
                            //         null);
                            // List<Employee> employees = this._employeeRepository.List(employeeSpec);
                            // if (employees.Count == 0)
                            // {
                            //     string err = string.Format("出库订单[{0}],关联经办人[{1}]不存在!", RequestCKLLOrder.AlyNumber,
                            //         RequestCKLLOrder.CreationBy);
                            //     throw new Exception(err);
                            // }

                            if (string.IsNullOrEmpty(RequestCKLLOrder.AlyDepCode))
                                throw new Exception("申请部门不能为空!");
                            OrganizationSpecification organizationSpec =
                                new OrganizationSpecification(Convert.ToInt32(RequestCKLLOrder.AlyDepCode), null, null, null);
                            List<Organization> alyOrgs = this._organizationRepository.List(organizationSpec);
                            if (alyOrgs.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联申请部门[{1}]不存在！", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.AlyDepCode);
                                throw new Exception(err);
                            }
                            Organization alyOrg = alyOrgs[0];

                            if (string.IsNullOrEmpty(RequestCKLLOrder.TransDepCode))
                                throw new Exception("领料部门不能为空!");
                            organizationSpec =
                                new OrganizationSpecification(Convert.ToInt32(RequestCKLLOrder.TransDepCode), null, null, null);
                            List<Organization> transOrgs = this._organizationRepository.List(organizationSpec);
                            if (transOrgs.Count == 0)
                            {
                                string err = string.Format("出库订单[{0}],关联领料部门[{1}]不存在！", RequestCKLLOrder.AlyNumber,
                                    RequestCKLLOrder.TransDepCode);
                                throw new Exception(err);
                            }
                            
                            //出库订单行里面的物料数量库存校验，防止有正在执行或者待处理的的退库订单冲突。
                            
                            SubOrderRowSpecification tkSubOrderRowSpec = new SubOrderRowSpecification(null,null,null,null,
                                new List<int>{Convert.ToInt32(ORDER_TYPE.出库退库)},ou.Id,warehouse.Id,null,null,null,null,null,null,
                                null,new List<int>{Convert.ToInt32(ORDER_STATUS.待处理),Convert.ToInt32(ORDER_STATUS.执行中)},null,null,null,null );

                            List<SubOrderRow> tkSubOrderRows = this._subOrderRowRepository.List(tkSubOrderRowSpec);
                            var tkSubOrderRowGroup = tkSubOrderRows.GroupBy(sr => new
                                {sr.SubOrder.OUId, sr.SubOrder.WarehouseId, sr.ReservoirAreaId, sr.MaterialDicId});
                            foreach (var tkGroup in tkSubOrderRowGroup)
                            {
                                var key = tkGroup.Key;
                                
                                //校验同一个OU、库存组织、子库区、物料编码的出库领料单行的物料数量是否小于剩余物料数量
                                var findSameRows = RequestCKLLOrder.RequestCKLLRows.FindAll(f=>Convert.ToInt32(f.InventoryCode)==key.ReservoirAreaId&&Convert.ToInt32(f.MaterialId)==key.MaterialDicId);
                                if (findSameRows.Count == 0) continue;
                                
                                // 领料单与退库单冲突
                                var totalTKOrderRowCount = tkGroup.Sum(m => m.PreCount);
                                
                                // 同一个OU、库存组织、子库区、物料编码 的库存现有数量总和
                                double totalMaterialCount = 0;
                                List<WarehouseMaterial> warehouseMaterials = null;
                                // 入库完成在货架上的物料
                                WarehouseMaterialSpecification inWarehouseMaterialSpec = new WarehouseMaterialSpecification(null,null,key.MaterialDicId,null,null,
                                    null,null,null,null,null,new List<int>{Convert.ToInt32(TRAY_STEP.入库完成)},null,key.OUId,key.WarehouseId,
                                    key.ReservoirAreaId,null,null,null,null,null);
                                
                                warehouseMaterials = this._warehouseMaterialRepository.List(inWarehouseMaterialSpec);
                                totalMaterialCount += warehouseMaterials.Sum(t => t.MaterialCount);
                                    
                                // 正在执行出库中、出库完成待确认的物料
                                WarehouseMaterialSpecification exeWarehouseMaterialSpec = new WarehouseMaterialSpecification(null,null,key.MaterialDicId,null,null,
                                    null,null,null,null,null,new List<int>{Convert.ToInt32(TRAY_STEP.待出库),Convert.ToInt32(TRAY_STEP.出库中未执行),Convert.ToInt32(TRAY_STEP.出库中已执行),
                                      Convert.ToInt32(TRAY_STEP.出库完成等待确认)},null,key.OUId,key.WarehouseId,
                                    key.ReservoirAreaId,null,null,null,null,null);
                                warehouseMaterials = this._warehouseMaterialRepository.List(exeWarehouseMaterialSpec);
                                totalMaterialCount += (warehouseMaterials.Sum(t => t.MaterialCount) + warehouseMaterials.Sum(t => t.OutCount.GetValueOrDefault()));
                                
                                // 出库完成确认完成的物料
                                WarehouseMaterialSpecification outEndWarehouseMaterialSpec = new WarehouseMaterialSpecification(null,null,key.MaterialDicId,null,null,
                                    null,null,null,null,null,new List<int>{Convert.ToInt32(TRAY_STEP.初始化)},null,key.OUId,key.WarehouseId,
                                    key.ReservoirAreaId,null,null,null,null,null);
                                warehouseMaterials = this._warehouseMaterialRepository.List(outEndWarehouseMaterialSpec);
                                totalMaterialCount +=  warehouseMaterials.Sum(t => t.MaterialCount);
                                
                                //同一个OU、库存组织、子库区、物料编码 剩余的数量总和 (库存物料数量-退库订单行物料数量)
                                double surplusTotalMaterialCount = totalMaterialCount - totalTKOrderRowCount;

                                if (surplusTotalMaterialCount < findSameRows.Sum(r => Convert.ToInt32(r.ReqQty)))
                                {
                                    throw new Exception(string.Format("前置出库订单头[{0}],行[{1}],行需求数量[{2}],退库数量[{3}],剩余数量[{4}],物料库存现有量不足无法出库!",
                                                                                RequestCKLLOrder.HeaderId,findSameRows[0].LineId,findSameRows[0].ReqQty,totalTKOrderRowCount,surplusTotalMaterialCount));
                                }
                            }
                            
                            #endregion

                            if (orders.Count > 0)
                            {
                                var srcOrder = orders[0];
                                // if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                                // {
                                //     string err = string.Format("出库订单[{0}]已经完成无法修改！", RequestCKLLOrder.AlyNumber);
                                //     throw new Exception(err);
                                //
                                // }
                                //
                                // if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                // {
                                //     string err = string.Format("出库订单[{0}]已经关闭无法修改！", RequestCKLLOrder.AlyNumber);
                                //     throw new Exception(err);
                                //
                                // }

                                if (RequestCKLLOrder.AlyStatusCode == "3")
                                {
                                    // if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.执行中) ||
                                    //     srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成) ||
                                    //     srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                    // {
                                    //     string err = string.Format("出库订单[{0}][{1}],无法撤销！", RequestCKLLOrder.AlyNumber,
                                    //         Enum.GetName(typeof(ORDER_STATUS), srcOrder.Status));
                                    //     throw new Exception(err);
                                    // }

                                    orderRows.ForEach(or =>
                                    {
                                        int reId = or.RelatedId.GetValueOrDefault();
                                        SubOrderRowSpecification subOrderRowSpecification =
                                            new SubOrderRowSpecification(null,
                                                null, null, reId,null, null, null, null,null,null, null, null,
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
                                            if (string.IsNullOrEmpty(eor.MaterialId))
                                                throw new Exception("物料Id不能为空!");
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
                                                string err = string.Format("出库订单[{0}],订单行[{1}],关联子库区Id[{2}]不存在！",
                                                    RequestCKLLOrder.AlyNumber, eor.LineNum, eor.InventoryCode);
                                                throw new Exception(err);
                                            }

                                            ReservoirArea area = areas[0];
                                            OrderRow addOrderRow = new OrderRow();
                                            
                                            addOrderRow.SourceId = Convert.ToInt32(eor.LineId);
                                            addOrderRow.RowNumber = eor.LineNum;
                                            addOrderRow.MaterialDicId = materialDic.Id;
                                            addOrderRow.UseFor = eor.UseFor;
                                            addOrderRow.PreCount = Convert.ToDouble(eor.ReqQty);
                                            addOrderRow.CancelCount = Convert.ToDouble(eor.CancelQty);
                                            addOrderRow.ReservoirAreaId = area.Id;
                                            addOrderRow.ExpenditureType = eor.ExpenditrueType;
                                            if (!string.IsNullOrEmpty(eor.OwnerId)) 
                                            {
                                                addOrderRow.OwnerId = Convert.ToInt32(eor.OwnerId);
                                                addOrderRow.OwnerType = eor.OwnerType;
                                            }

                                            if (!string.IsNullOrEmpty(eor.TaskId))
                                            {
                                                addOrderRow.EBSTaskId = Convert.ToInt32(eor.TaskId);
                                            }
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
                                                        null, null, reId,null, null, null, null,null,null, null, null,
                                                        null, null, null, null, null, null, null);
                                                List<SubOrderRow> subOrderRows =
                                                    this._subOrderRowRepository.List(subOrderRowSpecification);
                                                if (subOrderRows.Count > 0)
                                                    throw new Exception(string.Format("前置订单行[{0}]在WMS系统中已经拆分无法取消数量操作!",existRow.Id));
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
                                addOrder.SourceOrderType = RequestCKLLOrder.DocumentType;
                                if(!string.IsNullOrEmpty(RequestCKLLOrder.ItemId))
                                   addOrder.EBSProjectId = Convert.ToInt32(RequestCKLLOrder.ItemId);
                                addOrder.Memo = RequestCKLLOrder.Remark;
                                List<OrderRow> addOrderRows = new List<OrderRow>();
                                foreach (var eor in RequestCKLLOrder.RequestCKLLRows)
                                {
                                    if (string.IsNullOrEmpty(eor.MaterialId))
                                        throw new Exception("物料Id不能为空!");
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
                                    addOrderRow.PreCount = Convert.ToDouble(eor.ReqQty);
                                    addOrderRow.CancelCount = Convert.ToDouble(eor.CancelQty);
                                    addOrderRow.ReservoirAreaId = area.Id;
                                    addOrderRow.ExpenditureType = eor.ExpenditrueType;
                                    if (!string.IsNullOrEmpty(eor.TaskId)) 
                                    {
                                        addOrderRow.EBSTaskId = Convert.ToInt32(eor.TaskId);
                                    }
                                    addOrderRow.Memo = eor.Remark;
                                    if (!string.IsNullOrEmpty(eor.OwnerId))
                                    {
                                        addOrderRow.OwnerId = Convert.ToInt32(eor.OwnerId);
                                        addOrderRow.OwnerType = eor.OwnerType;
                                    }
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