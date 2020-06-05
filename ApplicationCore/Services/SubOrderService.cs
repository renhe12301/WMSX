using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Entities.SysManager;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class SubOrderService:ISubOrderService
    {

        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<SysConfig> _sysConfigRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;

        public SubOrderService(IAsyncRepository<SubOrder> subOrderRepository,
                               IAsyncRepository<SubOrderRow> subOrderRowRepository,
                               IAsyncRepository<LogRecord> logRecordRepository,
                               IAsyncRepository<OrderRow> orderRowRepository,
                               IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                               IAsyncRepository<MaterialDic> materialDicRepository,
                               IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                               IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                               IAsyncRepository<SysConfig> sysConfigRepository,
                               IAsyncRepository<Location> locationRepository,
                               IAsyncRepository<InOutTask> inOutTaskRepository
                               
                               
        )
        {
            this._subOrderRepository = subOrderRepository;
            this._subOrderRowRepository = subOrderRowRepository;
            this._logRecordRepository = logRecordRepository;
            this._orderRowRepository = orderRowRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._materialDicRepository = materialDicRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._sysConfigRepository = sysConfigRepository;
            this._locationRepository = locationRepository;
            this._inOutTaskRepository = inOutTaskRepository;
        }

        public async Task SortingOrder(int subOrderId, int subOrderRowId, double sortingCount, string trayCode, int areaId,
            string tag)
        {
            Guard.Against.Zero(subOrderId, nameof(subOrderId));
            Guard.Against.Zero(subOrderRowId, nameof(subOrderRowId));
            Guard.Against.Zero(sortingCount, nameof(sortingCount));
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));

            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        var subOrderSpec = new SubOrderSpecification(subOrderId, null, null,null, null,
                            null, null,null, null,null, null, null, null, null, null,
                            null, null, null);
                        var subOrders = this._subOrderRepository.List(subOrderSpec);
                        if (subOrders.Count == 0) throw new Exception(string.Format("订单[{0}],不存在！", subOrderId));
                        SubOrder subOrder = subOrders.First();
                        if (subOrder.Status != Convert.ToInt32(ORDER_STATUS.待处理) &&
                            subOrder.Status != Convert.ToInt32(ORDER_STATUS.执行中))
                            throw new Exception(string.Format("订单[{0}]状态必须为待处理或执行中！", subOrderId));
                        var subOrderRowSpec = new SubOrderRowSpecification(subOrderRowId, null, null, null,null, null,
                            null, null, null, null, null, null, null, null, null,
                            null, null);
                        var subOrderRows = this._subOrderRowRepository.List(subOrderRowSpec);
                        if (subOrderRows.Count == 0)
                            throw new Exception(string.Format("订单行编号[{0}],不存在！", subOrderRowId));
                        SubOrderRow subOrderRow = subOrderRows.First();
                        if (subOrderRow.Status != Convert.ToInt32(ORDER_STATUS.待处理) &&
                            subOrderRow.Status != Convert.ToInt32(ORDER_STATUS.执行中))
                            throw new Exception(string.Format("订单行[{0}]状态必须为待处理或执行中！", subOrderRowId));

                        var areaSpec = new ReservoirAreaSpecification(areaId, null, null, null, null, null);
                        var areas = this._reservoirAreaRepository.List(areaSpec);
                        if (areas.Count == 0) throw new Exception(string.Format("子库存编号[{0}],不存在！", areaId));
                        var area = areas[0];

                        var materialDicSpec =
                            new MaterialDicSpecification(subOrderRow.MaterialDicId, null, null, null, null);
                        var materialDics = this._materialDicRepository.List(materialDicSpec);
                        if (materialDics.Count == 0)
                            throw new Exception(string.Format("物料字典[{0}]),不存在！", subOrderRow.MaterialDicId));

                        if (area.WarehouseId != subOrder.WarehouseId)
                            throw new Exception(string.Format("当前订单[{0}]的库存组织和分拣托盘的库组织不一致,无法分拣！", subOrderId));

                        MaterialDic materialDic = materialDics[0];

                        var warehouseTraySpec = new WarehouseTraySpecification(null, trayCode, null, null, null, null,
                            null, null, null, null, null, null);

                        var whTrays = this._warehouseTrayRepository.List(warehouseTraySpec);
                        if (whTrays.Count > 0 && whTrays[0].TrayStep == Convert.ToInt32(TRAY_STEP.待入库))
                        {
                            var sortingTray = whTrays[0];
                            if (sortingTray.SubOrderRow.Id != subOrderRowId)
                                throw new Exception(string.Format("托盘[{0}]分拣的订单行[{1}]与当前订单行[{2}]不一致",
                                    sortingTray.TrayCode,
                                    sortingTray.SubOrderRow.Id, subOrderRowId));

                            var srcTrayCount = sortingTray.MaterialCount + sortingTray.OutCount;
                            subOrderRow.Sorting -= srcTrayCount;
                            var pOrderRow = subOrderRow.OrderRow;
                            pOrderRow.Sorting -= srcTrayCount;
                            if ((subOrderRow.Sorting + sortingCount) > subOrderRow.PreCount)
                            {
                                throw new Exception(string.Format("托盘分拣的总数量[{0}]大于订单行[{1}]数量",
                                    (subOrderRow.Sorting + sortingCount), sortingTray.SubOrderRow.Id,
                                    subOrderRowId));
                            }

                            double pOrderRowSorting = pOrderRow.Sorting.GetValueOrDefault();
                            pOrderRowSorting += sortingCount;
                            
                            double subOrderRowSorting = subOrderRow.Sorting.GetValueOrDefault();
                            subOrderRowSorting += sortingCount;
                            
                            pOrderRow.Sorting = pOrderRowSorting;
                            subOrderRow.Sorting = subOrderRowSorting;
                            //订单，订单行数量更新
                            if (subOrder.Status == Convert.ToInt32(ORDER_STATUS.待处理))
                            {
                                subOrder.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                            }

                            if (subOrderRow.Status == Convert.ToInt32(ORDER_STATUS.待处理))
                            {
                                subOrderRow.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                            }

                            sortingTray.MaterialCount = sortingCount + (sortingTray.OutCount.GetValueOrDefault()<0?sortingTray.OutCount.GetValueOrDefault()*-1:sortingTray.OutCount.GetValueOrDefault());
                            
                            WarehouseMaterialSpecification warehouseMaterialSpec =
                                new WarehouseMaterialSpecification(
                                    null,
                                    null, null, null, null, null, sortingTray.Id, null,
                                    null, null, null, null, null, null, null,
                                    null, null, null, null, null);

                            List<WarehouseMaterial> oldMaterials =
                                this._warehouseMaterialRepository.List(warehouseMaterialSpec);

                            if (oldMaterials.Count > 0)
                            {
                                oldMaterials[0].MaterialCount = sortingTray.MaterialCount;
                                this._warehouseMaterialRepository.Update(oldMaterials[0]);
                            }

                            this._warehouseTrayRepository.Update(sortingTray);
                            this._subOrderRepository.Update(subOrder);
                            this._subOrderRowRepository.Update(subOrderRow);
                            this._orderRowRepository.Update(pOrderRow);
                            
                        }
                        else
                        {
                            if (whTrays.Count > 0 && whTrays[0].TrayStep != Convert.ToInt32(TRAY_STEP.初始化))
                                throw new Exception(string.Format("托盘状态[{0}]未初始化,无法分拣！", trayCode));

                            DateTime now = DateTime.Now;
                            if ((sortingCount + subOrderRow.Sorting.GetValueOrDefault()) > subOrderRow.PreCount)
                                throw new Exception(string.Format("已分拣数量总和大于订单行[{0}]数量", subOrderRowId));

                            WarehouseTray warehouseTray = null;
                            if (whTrays.Count == 0)
                            {
                                warehouseTray = new WarehouseTray
                                {
                                    TrayCode = trayCode,
                                    CreateTime = now,
                                    SubOrderId = subOrderId,
                                    SubOrderRowId = subOrderRowId,
                                    MaterialCount = sortingCount,
                                    Price = subOrderRow.Price,
                                    Amount = subOrderRow.Price * sortingCount,
                                    TrayStep = Convert.ToInt32(TRAY_STEP.待入库),
                                    ReservoirAreaId = areaId,
                                    Carrier = Convert.ToInt32(TRAY_CARRIER.货位),
                                    OutCount = 0,
                                    OUId = area.OUId,
                                    WarehouseId = area.WarehouseId
                                };
                                var addTray = this._warehouseTrayRepository.Add(warehouseTray);
                                WarehouseMaterial warehouseMaterial = new WarehouseMaterial
                                {
                                    SubOrderId = subOrderId,
                                    SubOrderRowId = subOrderRowId,
                                    CreateTime = now,
                                    WarehouseTrayId = addTray.Id,
                                    MaterialCount = sortingCount,
                                    OutCount = 0,
                                    MaterialDicId = materialDic.Id,
                                    Price = subOrderRow.Price,
                                    Amount = subOrderRow.Price * sortingCount,
                                    WarehouseId = area.WarehouseId,
                                    ReservoirAreaId = areaId,
                                    OUId = area.OUId,
                                    Carrier = Convert.ToInt32(TRAY_CARRIER.货位),
                                    SupplierId = subOrder.SupplierId,
                                    SupplierSiteId = subOrder.SupplierSiteId
                                };
                                this._warehouseMaterialRepository.Add(warehouseMaterial);
                            }
                            else
                            {

                                warehouseTray = whTrays[0];
                                if (warehouseTray.WarehouseId != subOrder.WarehouseId)
                                    throw new Exception("托盘与订单库存组织不一致无法分拣!");
                                if (warehouseTray.ReservoirAreaId != subOrderRow.ReservoirAreaId)
                                    throw new Exception("托盘与订单行库区不一致无法分拣!");

                                WarehouseMaterialSpecification warehouseMaterialSpec =
                                    new WarehouseMaterialSpecification(
                                        null,
                                        null, null, null, null, null, warehouseTray.Id, null,
                                        null, null, null, null, null, null, null,
                                        null, null, null, null, null);

                                List<WarehouseMaterial> oldMaterials =
                                    this._warehouseMaterialRepository.List(warehouseMaterialSpec);

                                double oldCount = 0;
                                if (warehouseTray.MaterialCount > 0)
                                {
                                    if (oldMaterials[0].MaterialDicId != subOrderRow.MaterialDicId)
                                        throw new Exception("分拣物料与原有托盘上的物料不一致！");
                                    oldCount = warehouseTray.MaterialCount;
                                }

                                warehouseTray.MaterialCount = oldCount + sortingCount;
                                warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.待入库);
                                warehouseTray.CreateTime = DateTime.Now;

                                warehouseTray.SubOrderId = subOrderId;
                                warehouseTray.SubOrderRowId = subOrderRowId;
                                warehouseTray.OutCount = oldCount > 0 ? -1 * oldCount : 0;
                                this._warehouseTrayRepository.Update(warehouseTray);
                                WarehouseMaterial warehouseMaterial = null;

                                if (oldMaterials.Count > 0)
                                    warehouseMaterial = oldMaterials[0];
                                else warehouseMaterial = new WarehouseMaterial();

                                warehouseMaterial.SubOrderId = subOrderId;
                                warehouseMaterial.SubOrderRowId = subOrderRowId;
                                warehouseMaterial.CreateTime = now;
                                warehouseMaterial.WarehouseTrayId = warehouseTray.Id;
                                warehouseMaterial.MaterialCount = oldCount + sortingCount;
                                warehouseMaterial.OutCount = warehouseTray.OutCount;
                                warehouseMaterial.MaterialDicId = materialDic.Id;
                                warehouseMaterial.Price = subOrderRow.Price;
                                warehouseMaterial.Amount = subOrderRow.Price * (oldCount + sortingCount);
                                warehouseMaterial.WarehouseId = area.WarehouseId;
                                warehouseMaterial.ReservoirAreaId = areaId;
                                warehouseMaterial.OUId = area.OUId;
                                warehouseMaterial.SupplierId = subOrder.SupplierId;
                                warehouseMaterial.SupplierSiteId = subOrder.SupplierSiteId;

                                if (oldMaterials.Count > 0)
                                    this._warehouseMaterialRepository.Update(warehouseMaterial);
                                else
                                    this._warehouseMaterialRepository.Add(warehouseMaterial);
                            }
                            
                            var pOrderRow = subOrderRow.OrderRow;
                            double pOrderRowSorting = pOrderRow.Sorting.GetValueOrDefault();
                            pOrderRowSorting += sortingCount;
                            
                            double subOrderRowSorting = subOrderRow.Sorting.GetValueOrDefault();
                            subOrderRowSorting += sortingCount;
                            
                            pOrderRow.Sorting = pOrderRowSorting;
                            subOrderRow.Sorting = subOrderRowSorting;
                            //订单，订单行数量更新
                            if (subOrder.Status == Convert.ToInt32(ORDER_STATUS.待处理))
                            {
                                subOrder.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                            }

                            if (subOrderRow.Status == Convert.ToInt32(ORDER_STATUS.待处理))
                            {
                                subOrderRow.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                            }
                            this._subOrderRepository.Update(subOrder);
                            this._subOrderRowRepository.Update(subOrderRow);
                            this._orderRowRepository.Update(pOrderRow);
                        }
                        
                       
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                            LogDesc = string.Format("订单[{0}]出库,订单行[{1}],分拣数量[{2}],分拣托盘[{3}]",
                                subOrder.Id,
                                subOrderRow.Id,
                                subOrderRow.Sorting,
                                trayCode),
                            Founder = tag,
                            CreateTime = DateTime.Now
                        });
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                            LogDesc = ex.ToString(),
                            CreateTime = DateTime.Now
                        });
                        throw ex;
                    }
                }
            }
        }

        public async Task CreateOrder(SubOrder order)
        {
            Guard.Against.Null(order, nameof(order));
            Guard.Against.Zero(order.SubOrderRow.Count, nameof(order.SubOrderRow));
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        List<OrderRow> updOrderRows = new List<OrderRow>();
                        foreach (var subOrderRow in order.SubOrderRow)
                        {
                            Guard.Against.Null(subOrderRow.OrderRowId, nameof(subOrderRow.OrderRowId));
                            if (subOrderRow.OrderRowId.HasValue)
                            {
                                OrderRowSpecification orderRowSpecification = new OrderRowSpecification(
                                    subOrderRow.OrderRowId,
                                    null, null, null, null, null, null, 
                                    null, null, null,null,null,null
                                    ,null,null,null);
                                List<OrderRow> orderRows =
                                    this._orderRowRepository.List(orderRowSpecification);
                                Guard.Against.Zero(orderRows.Count, nameof(orderRows));
                                OrderRow orderRow = orderRows.First();
                                if(subOrderRow.PreCount<=0)
                                    throw new Exception(string.Format("订单行[{0}]数量[{1}],不是有效的数字!",subOrderRow.OrderRowId,subOrderRow.PreCount));
                                if ( subOrderRow.PreCount>(orderRow.PreCount - orderRow.Expend.GetValueOrDefault()))
                                    throw new Exception(string.Format("行数量大于前置订单行[{0}]剩余数量[{1}]",
                                        subOrderRow.OrderRowId, orderRow.PreCount - orderRow.Expend.GetValueOrDefault()));
                                double expend = orderRow.Expend.GetValueOrDefault();
                                expend += subOrderRow.PreCount;
                                orderRow.Expend = expend;
                                updOrderRows.Add(orderRow);
                            }
                        }

                        if (updOrderRows.Count > 0)
                            this._orderRowRepository.Update(updOrderRows);
                        SubOrder newSubOrder = this._subOrderRepository.Add(order);
                        order.SubOrderRow.ForEach(or => or.SubOrderId = newSubOrder.Id);
                        this._subOrderRowRepository.Add(order.SubOrderRow);
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                            LogDesc = string.Format("新建后置订单[{0}]!",newSubOrder.Id),
                            CreateTime = DateTime.Now
                        });
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                            LogDesc = ex.Message,
                            CreateTime = DateTime.Now
                        });
                        throw ex;
                    }
                }
            }

        }

        public async Task ScrapOrder(SubOrder order)
        {
            Guard.Against.Null(order, nameof(order));
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {

                        SubOrderSpecification subOrderSpecification = new SubOrderSpecification(order.Id, null,
                            null,null, null, null,null,null, null, null, null, null, null,
                            null, null, null, null, null);
                        List<SubOrder> subOrders =  this._subOrderRepository.List(subOrderSpecification);
                        Guard.Against.Zero(subOrders.Count, nameof(subOrders));
                        SubOrder subOrder = subOrders.First();

                        if (subOrder.Status != Convert.ToInt32(ORDER_STATUS.待处理))
                            throw new Exception(string.Format("订单[{0}]状态必须为待处理才能作废!", subOrder.Id));
                        subOrder.Status = Convert.ToInt32(ORDER_STATUS.关闭);

                        SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(null,
                            subOrder.Id, null, null,null, null, null, null, null, null
                            , null, null, null, null, null, null, null);
                        List<SubOrderRow> subOrderRows =
                             this._subOrderRowRepository.List(subOrderRowSpecification);
                        List<OrderRow> updOrderRows = new List<OrderRow>();
                        foreach (var subOrderRow in subOrderRows)
                        {
                            if (subOrderRow.Status != Convert.ToInt32(ORDER_STATUS.待处理))
                                throw new Exception(string.Format("订单行[{0}]状态必须为待处理才能作废!", subOrderRow.Id));
                            subOrderRow.Status = Convert.ToInt32(ORDER_STATUS.关闭);
                            if (subOrderRow.OrderRowId.HasValue)
                            {
                                OrderRowSpecification orderRowSpecification = new OrderRowSpecification(
                                    subOrderRow.OrderRowId,
                                    null, null, null, null, null, null,
                                    null, null, null,null,null,null,null
                                    ,null,null);
                                List<OrderRow> orderRows =
                                     this._orderRowRepository.List(orderRowSpecification);
                                Guard.Against.Zero(orderRows.Count, nameof(orderRows));
                                OrderRow orderRow = orderRows.First();
                                orderRow.Expend -= subOrderRow.PreCount;
                                updOrderRows.Add(orderRow);
                            }
                        }

                        if (updOrderRows.Count > 0)
                            this._orderRowRepository.Update(updOrderRows);
                        this._subOrderRowRepository.Update(subOrderRows);
                        this._subOrderRepository.Update(subOrder);
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                            LogDesc = ex.Message,
                            CreateTime = DateTime.Now
                        });
                        throw ex;
                    }
                }
            }
        }

        public async Task ScrapOrderRow(List<SubOrderRow> subOrderRows)
        {
            Guard.Against.Zero(subOrderRows.Count, nameof(subOrderRows));
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        List<OrderRow> updOrderRows = new List<OrderRow>();
                        foreach (var sr in subOrderRows)
                        {
                            SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(sr.Id,
                                null
                                , null, null,null, null, null, null, null, null
                                , null, null, null, null, null, null, null);
                            List<SubOrderRow> findSubOrderRows =
                                this._subOrderRowRepository.List(subOrderRowSpecification);
                            SubOrderRow subOrderRow = findSubOrderRows.First();
                            if (subOrderRow.Status != Convert.ToInt32(ORDER_STATUS.待处理))
                                throw new Exception(string.Format("订单行[{0}]状态必须为待处理才能作废!", subOrderRow.Id));
                            sr.Status = Convert.ToInt32(ORDER_STATUS.关闭);
                            if (subOrderRow.OrderRowId.HasValue)
                            {
                                OrderRowSpecification orderRowSpecification = new OrderRowSpecification(
                                    subOrderRow.OrderRowId,null, null, null, null, null, null, null,
                                    null, null,null,null,null,null,null,null);
                                List<OrderRow> orderRows = this._orderRowRepository.List(orderRowSpecification);
                                Guard.Against.Zero(orderRows.Count, nameof(orderRows));
                                OrderRow orderRow = orderRows.First();
                                orderRow.Expend -= subOrderRow.PreCount;
                                updOrderRows.Add(orderRow);
                            }
                        }

                        if (updOrderRows.Count > 0)
                            this._orderRowRepository.Update(updOrderRows);
                        this._subOrderRowRepository.Update(subOrderRows);
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                            LogDesc = ex.Message,
                            CreateTime = DateTime.Now
                        });
                        throw ex;
                    }
                }
            }
        }

        public async Task OutConfirm(int subOrderId)
        {
            Guard.Against.Zero(subOrderId, nameof(subOrderId));
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        SubOrderSpecification subOrderSpecification = new SubOrderSpecification(subOrderId, null, null,null,
                            null, null,null,null, null, null, null, null, null, null, null,
                            null, null, null);
                        List<SubOrder> subOrders = this._subOrderRepository.List(subOrderSpecification);
                        Guard.Against.Zero(subOrders.Count, nameof(subOrders));
                        SubOrder subOrder = subOrders.First();
                        if (subOrder.Status != Convert.ToInt32(ORDER_STATUS.待处理))
                            throw new Exception(string.Format("只能处理状态为[待处理]的订单,当前的订单状态为[{0}]",
                                Enum.GetName(typeof(ORDER_STATUS), subOrder.Status)));
                        SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(null,
                            subOrderId,
                            null, null,null, null, null, null, null, null,
                            null, null, null, null, null, null, null);
                        List<SubOrderRow> subOrderRows =
                            this._subOrderRowRepository.List(subOrderRowSpecification);
                        Guard.Against.Zero(subOrderRows.Count, nameof(subOrderRows));
                        List<SysConfig> sysConfigs = this._sysConfigRepository.ListAll();
                        SysConfig config = sysConfigs.Find(s => s.KName == "出入库唯一校验");
                        if (config.KVal == "1")
                        {
                           
                            SubOrderRow subOrderRow = subOrderRows.First();
                            ReservoirArea area = subOrderRow.ReservoirArea;
                            LocationSpecification locationSpecification = new LocationSpecification(null, null,
                                null, null, null, null, null, area.Id, null,
                                null, null, null, null, null);
                            List<Location> locations =  this._locationRepository.List(locationSpecification);
                            Guard.Against.Zero(locations.Count, nameof(locations));
                            Location location = locations.First();
                            PhyWarehouse phyWarehouse = location.PhyWarehouse;
                            InOutTaskSpecification inOutTaskSpecification = new InOutTaskSpecification(null, null,null,null,
                                new List<int>
                                {
                                    Convert.ToInt32(TASK_STATUS.待处理),
                                    Convert.ToInt32(TASK_STATUS.执行中)
                                }, null,
                                new List<int> {Convert.ToInt32(TASK_TYPE.物料入库), Convert.ToInt32(TASK_TYPE.空托盘入库)}, null,
                                null, null, null, phyWarehouse.Id, null, null, null, null);
                            List<InOutTask> inOutTasks =
                                 this._inOutTaskRepository.List(inOutTaskSpecification);
                            if (inOutTasks.Count > 0)
                                throw new Exception("当前系统配置了[出入库唯一性校验],当前有正在执行的入库任务,无法执行出库操作!");
                        }

                        subOrder.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                        subOrder.IsRead = Convert.ToInt32(ORDER_READ.未读);
                        this._subOrderRepository.Update(subOrder);
                        subOrderRows.ForEach(r=>r.Status = Convert.ToInt32(ORDER_STATUS.执行中));
                        this._subOrderRowRepository.Update(subOrderRows);
                        
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                            LogDesc = "出库订单确认!",
                            CreateTime = DateTime.Now
                        });
                        
                        scope.Complete();

                    }
                    catch (Exception ex)
                    {
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                            LogDesc = ex.Message,
                            CreateTime = DateTime.Now
                        });
                        throw ex;
                    }
                }
            }
        }
    }
}