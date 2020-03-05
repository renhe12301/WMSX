using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;
using ApplicationCore.Misc;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.StockManager;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<InOutRecord> _inOutTaskRepository;
        private readonly IAsyncRepository<OrderRowBatch> _orderRowBatchRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        public OrderService(IAsyncRepository<Order> orderRepository,
                               IAsyncRepository<OrderRow> orderRowRepository,
                               IAsyncRepository<MaterialDic> materialDicRepository,
                               IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                               IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                               IAsyncRepository<ReservoirArea> reservoirRepository,
                               IAsyncRepository<InOutRecord> inOutRecordRepository,
                               IAsyncRepository<OrderRowBatch> orderRowBatchRepository,
                               IAsyncRepository<LogRecord> logRecordRepository
                               )
        {
            this._orderRepository = orderRepository;
            this._orderRowRepository = orderRowRepository;
            this._materialDicRepository = materialDicRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._reservoirAreaRepository = reservoirRepository;
            this._inOutTaskRepository = inOutRecordRepository;
            this._orderRowBatchRepository = orderRowBatchRepository;
            this._logRecordRepository = logRecordRepository;
        }


        public async Task<int> CreateOutOrder(Order order)
        {
            Guard.Against.Null(order, nameof(order));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {

                    OrderSpecification orderSpec = new OrderSpecification(null, order.OrderNumber, null,
                        null,
                        null, null, null, null, null, null, null,
                        null, null, null, null, null);
                    List<Order> orders = await this._orderRepository.ListAsync(orderSpec);

                    OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null,
                        order.OrderNumber, null, null, null, null, null);
                    List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);

                    //对占用订单行里面物料数量进行校验
                    OrderRowSpecification checkOrderRowSpec = new OrderRowSpecification(null, null, null,
                        new List<int> {Convert.ToInt32(ORDER_STATUS.待处理), Convert.ToInt32(ORDER_STATUS.执行中)}, null,
                        null, null, null);
                    List<OrderRow> checkOrderRows = await this._orderRowRepository.ListAsync(checkOrderRowSpec);
                    List<OrderRow> tkOrderRows = checkOrderRows
                        .Where(or => or.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库)).ToList();
                    StringBuilder errorSB = new StringBuilder();

                    WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                        null,
                        null, null, null, null, null, null, null,
                        null, new List<int>() {Convert.ToInt32(TRAY_STEP.入库完成), Convert.ToInt32(TRAY_STEP.初始化)},
                        null, order.OUId, order.WarehouseId, null, null, null);
                    List<WarehouseMaterial> allWarehouseMaterials =
                        await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);
                    if (allWarehouseMaterials.Count > 0)
                    {
                        order.OrderRow.ForEach((o) =>
                        {
                            List<WarehouseMaterial> warehouseMaterials = allWarehouseMaterials.Where(w =>
                                    w.MaterialDicId == o.MaterialDicId && w.ReservoirAreaId == o.ReservoirAreaId)
                                .ToList();
                            int stockCount = warehouseMaterials.Sum(m => m.MaterialCount);
                            int occCount = tkOrderRows.Sum(or => or.MaterialDicId = o.MaterialDicId);
                            int remainingCount = stockCount - occCount;
                            if (remainingCount < o.PreCount)
                                errorSB.Append(string.Format("物料Id[{0}],物料名称[{1}],库存不足,出库失败！\n", o.MaterialDicId,
                                    o.MaterialDic.MaterialName));

                        });
                        if (!string.IsNullOrEmpty(errorSB.ToString()))
                            throw new Exception(errorSB.ToString());
                    }

                    if (orders.Count > 0)
                    {
                        var srcOrder = orders[0];
                        if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                            throw new Exception(string.Format("订单[{0}]已经完成无法修改！", srcOrder.OrderNumber));

                        if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                            throw new Exception(string.Format("订单[{0}]已经关闭无法修改！", srcOrder.OrderNumber));
                        if (order.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                        {
                            if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.执行中))
                                throw new Exception(string.Format("订单[{0}]正在执行无法关闭！", order.OrderNumber));
                            List<Order> updOrders = new List<Order>();
                            List<OrderRow> updOrderRows = new List<OrderRow>();
                            int exeCount = orderRows.Count(r =>
                                r.OrderId == srcOrder.Id && r.Status == Convert.ToInt32(ORDER_STATUS.执行中));
                            if (exeCount > 0)
                                throw new Exception(string.Format("订单[{0}],正在执行中的订单行[{1}]个,无法关闭！",
                                    order.OrderNumber, exeCount));
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
                                    this._logRecordRepository.Add(new LogRecord
                                    {
                                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                                        LogDesc = "关闭订单[{0}]" + srcOrder.OrderNumber,
                                        CreateTime = DateTime.Now,
                                        Founder = order.Tag
                                    });
                                    scope.Complete();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }

                        }
                        else
                        {
                            List<OrderRow> addOrderRows = new List<OrderRow>();
                            List<OrderRow> updOrderRows = new List<OrderRow>();
                            order.OrderRow.ForEach(async (eor) =>
                            {
                                var existRow = orderRows.Find(r => r.RowNumber == eor.RowNumber);
                                if (existRow == null)
                                {
                                    MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(
                                        Convert.ToInt32(eor.MaterialDicId),
                                        null, null, null, null);
                                    List<MaterialDic> materialDics =
                                        await this._materialDicRepository.ListAsync(materialDicSpec);
                                    if (materialDics.Count == 0)
                                        throw new Exception(string.Format("订单[{0}],订单行[{1}],关联物料Id[{2}]不存在！",
                                            order.OrderNumber, eor.RowNumber, eor.MaterialDicId));
                                    MaterialDic materialDic = materialDics[0];

                                    OrderRow addOrderRow = new OrderRow
                                    {
                                        OrderId = srcOrder.Id,
                                        RowNumber = eor.RowNumber,
                                        MaterialDicId = materialDic.Id,
                                        PreCount = Convert.ToInt32(eor.PreCount),
                                        Price = Convert.ToInt32(eor.Price),
                                        Amount = Convert.ToInt32(eor.Amount)
                                    };
                                    addOrderRows.Add(addOrderRow);
                                }
                                else
                                {
                                    if (existRow.Status != Convert.ToInt32(ORDER_STATUS.完成) &&
                                        existRow.Status != Convert.ToInt32(ORDER_STATUS.关闭))
                                    {
                                        if (eor.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                        {
                                            if (existRow.Status == Convert.ToInt32(ORDER_STATUS.执行中))
                                                throw new Exception(string.Format(
                                                    "修改订单[{0}],关闭订单行[{1}],关闭失败,订单行执行中",
                                                    order.OrderNumber, eor.RowNumber));
                                            existRow.Status = Convert.ToInt32(ORDER_STATUS.关闭);
                                        }

                                        if (Convert.ToInt32(eor.PreCount) < (existRow.PreCount - existRow.Sorting))
                                            throw new Exception(string.Format(
                                                "修改订单[{0}],订单行[{1}],修改数量大于剩余数量,已处理[{2}],剩余[{3}]",
                                                order.OrderNumber, eor.RowNumber, existRow.Sorting,
                                                existRow.PreCount - existRow.Sorting));

                                        existRow.PreCount = Convert.ToInt32(eor.PreCount);
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
                                    StringBuilder sb = new StringBuilder(string.Format("修改订单[{0}]\n", srcOrder.Id));
                                    if (addOrderRows.Count > 0)
                                        sb.Append(string.Format("新增订单行[{0}]\n",
                                            string.Join(',', addOrderRows.ConvertAll(r => r.Id))));
                                    if (updOrderRows.Count > 0)
                                        sb.Append(string.Format("修改订单行[{0}]\n",
                                            string.Join(',', updOrderRows.ConvertAll(r => r.Id))));

                                    this._logRecordRepository.Add(new LogRecord
                                    {
                                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                                        LogDesc = sb.ToString(),
                                        CreateTime = DateTime.Now,
                                        Founder = order.Tag
                                    });
                                    scope.Complete();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            try
                            {
                                this._orderRepository.Add(order);
                                order.OrderRow.ForEach(om => om.OrderId = order.Id);
                                this._orderRowRepository.Add(order.OrderRow);
                                this._logRecordRepository.Add(new LogRecord
                                {
                                    LogType = Convert.ToInt32(LOG_TYPE.WebService调用日志),
                                    LogDesc = string.Format("新增入库订单[{0}]\n新增入库订单行[{1}]", order.Id,
                                        string.Join(',', order.OrderRow.ConvertAll(r => r.Id))),
                                    CreateTime = DateTime.Now,
                                    Founder = order.Tag
                                });
                                scope.Complete();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    await this._logRecordRepository.AddAsync(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.InnerException==null?ex.StackTrace:ex.InnerException.ToString(),
                        CreateTime = DateTime.Now
                    });
                }
                return order.Id;
            }
        }

        public async Task SortingOrder(int orderId, int orderRowId, int sortingCount,int badCount,string trayCode,
                                       int areaId, string tag)
        {
            Guard.Against.Zero(orderId, nameof(orderId));
            Guard.Against.Zero(orderRowId, nameof(orderRowId));
            Guard.Against.Zero(sortingCount, nameof(sortingCount));
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                var orderSpec = new OrderSpecification(orderId, null, null, null, null,
                    null, null, null, null, null, null, null,
                    null, null, null, null);
                var orders = await this._orderRepository.ListAsync(orderSpec);
                if (orders.Count == 0) throw new Exception(string.Format("订单编号[{0}],不存在！", orderId));
                Order order = orders[0];
                var orderRowSpec = new OrderRowSpecification(orderRowId, null,null, null, null, null, null, null);
                var orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                if (orderRows.Count == 0) throw new Exception(string.Format("订单行编号[{0}],不存在！", orderRowId));
                OrderRow orderRow = orderRows[0];
                var areaSpec = new ReservoirAreaSpecification(areaId, null, null, null, null, null);
                var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
                if (areas.Count == 0) throw new Exception(string.Format("子库存编号[{0}],不存在！", areaId));
                var area = areas[0];
                var materialDicSpec = new MaterialDicSpecification(orderRow.MaterialDicId, null, null, null, null);
                var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
                if (materialDics.Count == 0)
                    throw new Exception(string.Format("物料字典[{0}]),不存在！", orderRow.MaterialDicId));

                if (area.WarehouseId != order.WarehouseId)
                    throw new Exception(string.Format("当前订单的库存组织和分拣托盘的库组织不一致,无法分拣！"));

                MaterialDic materialDic = materialDics[0];

                if (sortingCount > (orderRow.PreCount-orderRow.BadCount)) throw new Exception("分拣数量不能大于申请数量！");
                int surplusCount = orderRow.PreCount - (orderRow.Sorting.GetValueOrDefault()+orderRow.BadCount.GetValueOrDefault());
                if (sortingCount > surplusCount)
                    throw new Exception(string.Format("已经分拣了{0}个,最多还能分拣{1}个", orderRow.Sorting, surplusCount));

                var warehouseTraySpec = new WarehouseTraySpecification(null, trayCode,
                    null, null, null, null,
                    null, null, null, null, null);

                var whTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                if (whTrays.Count > 0 && whTrays[0].TrayStep != Convert.ToInt32(TRAY_STEP.初始化))
                    throw new Exception(string.Format("托盘[{0}]未初始化,无法分拣！", trayCode));
                if (whTrays.Count > 0 && whTrays[0].TrayStep == Convert.ToInt32(TRAY_STEP.待入库))
                    throw new Exception(string.Format("托盘[{0}]已经分拣,无法再次分拣！", trayCode));

                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                    null, null, null, null, trayCode, null, null,
                    null, null, null, null, null, null, null,null,null);

                List<WarehouseMaterial> oldMaterials =
                    await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);

                DateTime now = DateTime.Now;
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        orderRow.Sorting += sortingCount;
                        orderRow.BadCount += badCount;
                        this._orderRowRepository.Update(orderRow);
                        WarehouseTray warehouseTray = null;
                        if (whTrays.Count == 0)
                        {
                            warehouseTray = new WarehouseTray
                            {
                                TrayCode = trayCode,
                                CreateTime = now,
                                OrderId = orderId,
                                OrderRowId = orderRowId,
                                MaterialCount = sortingCount,
                                Price = orderRow.Price,
                                Amount = orderRow.Price * sortingCount,
                                TrayStep = Convert.ToInt32(TRAY_STEP.待入库),
                                ReservoirAreaId = areaId,
                                OutCount = 0,
                                OUId = area.OUId
                            };
                            var addTray = this._warehouseTrayRepository.Add(warehouseTray);
                            WarehouseMaterial warehouseMaterial = new WarehouseMaterial
                            {
                                OrderId = orderId,
                                OrderRowId = orderRowId,
                                CreateTime = now,
                                WarehouseTrayId = addTray.Id,
                                MaterialCount = sortingCount,
                                MaterialDicId = materialDic.Id,
                                Price = orderRow.Price,
                                Amount = orderRow.Price * sortingCount,
                                WarehouseId = area.WarehouseId,
                                ReservoirAreaId = areaId,
                                OUId = area.OUId,
                                SupplierId = order.SupplierId,
                                SupplierSiteId = order.SupplierSiteId
                            };
                            this._warehouseMaterialRepository.Add(warehouseMaterial);
                        }
                        else
                        {
                            warehouseTray = whTrays[0];
                            int oldCount = 0;
                            if (warehouseTray.MaterialCount > 0)
                            {
                                if (oldMaterials[0].MaterialDicId != orderRow.MaterialDicId)
                                    throw new Exception("分拣物料与原有托盘上的物料不一致！");
                                oldCount = warehouseTray.MaterialCount;
                            }

                            warehouseTray.MaterialCount = oldCount + sortingCount;
                            warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.待入库);
                            warehouseTray.CreateTime = DateTime.Now;

                            warehouseTray.OrderId = orderId;
                            warehouseTray.OrderRowId = orderRowId;
                            warehouseTray.OutCount = oldCount > 0 ? -1 * oldCount : 0;
                            this._warehouseTrayRepository.Update(warehouseTray);
                            WarehouseMaterial warehouseMaterial = null;

                            if (oldMaterials.Count > 0)
                                warehouseMaterial = oldMaterials[0];
                            else warehouseMaterial = new WarehouseMaterial();

                            warehouseMaterial.OrderId = orderId;
                            warehouseMaterial.OrderRowId = orderRowId;
                            warehouseMaterial.CreateTime = now;
                            warehouseMaterial.WarehouseTrayId = warehouseTray.Id;
                            warehouseMaterial.MaterialCount = oldCount + sortingCount;
                            warehouseMaterial.MaterialDicId = materialDic.Id;
                            warehouseMaterial.Price = orderRow.Price;
                            warehouseMaterial.Amount = orderRow.Price * sortingCount;
                            warehouseMaterial.WarehouseId = area.WarehouseId;
                            warehouseMaterial.ReservoirAreaId = areaId;
                            warehouseMaterial.OUId = area.OUId;
                            warehouseMaterial.SupplierId = order.SupplierId;
                            warehouseMaterial.SupplierSiteId = order.SupplierSiteId;

                            if (oldMaterials.Count > 0)
                                this._warehouseMaterialRepository.Update(warehouseMaterial);
                            else
                                this._warehouseMaterialRepository.Add(warehouseMaterial);
                        }

                        //订单，订单行数量更新
                        if (order.Status == Convert.ToInt32(ORDER_STATUS.待处理))
                        {
                            order.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                            this._orderRepository.Update(order);
                        }

                        if (orderRow.Status == Convert.ToInt32(ORDER_STATUS.待处理))
                        {
                            orderRow.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                        }

                        orderRow.Sorting += sortingCount;
                        this._orderRowRepository.Update(orderRow);

                        InOutRecord inOutRecord = new InOutRecord
                        {
                            TrayCode = trayCode,
                            OrderId = orderId,
                            OrderRowId = orderRowId,
                            OUId = area.OUId,
                            WarehouseId = area.WarehouseId,
                            ReservoirAreaId = areaId,
                            MaterialDicId = materialDic.Id,
                            BadCount = badCount,
                            InOutCount = warehouseTray.MaterialCount + warehouseTray.OutCount.GetValueOrDefault(),
                            IsRead = 0,
                            CreateTime = DateTime.Now,
                            Type = 0,
                            Status = Convert.ToInt32(TASK_STATUS.待处理)
                        };
                        this._inOutTaskRepository.Add(inOutRecord);
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                            LogDesc = string.Format("订单[{0}]出库,订单行[{1}],分拣数量[{2}],分拣托盘[{3}]",
                                orderRow.OrderId,
                                orderRow.Id,
                                orderRow.Sorting,
                                trayCode),
                            Founder = tag,
                            CreateTime = DateTime.Now
                        });
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

        }

        public async Task OrderOut(int orderId, int orderRowId, int areaId, int sortingCount, int type, string tag)
        {
            Guard.Against.Zero(orderId, nameof(orderId));
            Guard.Against.Zero(orderRowId, nameof(orderRowId));
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.Zero(sortingCount, nameof(sortingCount));

            var orderSpec = new OrderSpecification(orderId, null, null, null, null,
                null, null, null, null, null, null, null,
                null, null, null, null);
            var orders = await this._orderRepository.ListAsync(orderSpec);
            if (orders.Count == 0) throw new Exception(string.Format("订单编号[{0}],不存在！", orderId));
            Order order = orders[0];
            var orderRowSpec = new OrderRowSpecification(orderRowId, null, null, null, null, null, null, null);
            var orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
            if (orderRows.Count == 0) throw new Exception(string.Format("订单行编号[{0}],不存在！", orderRowId));
            OrderRow orderRow = orderRows[0];
            var areaSpec = new ReservoirAreaSpecification(areaId, null, null, null, null, null);
            var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
            if (areas.Count == 0) throw new Exception(string.Format("子库存编号[{0}],不存在！", areaId));
            var area = areas[0];

            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                if ((orderRow.PreCount - orderRow.CancelCount - orderRow.Sorting) > sortingCount)
                    throw new Exception(string.Format("出库数量[{0}]大于订单行申请数量[{1}]！",
                        sortingCount, orderRow.PreCount - orderRow.CancelCount));
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        //订单，订单行数量更新
                        if (order.Status == Convert.ToInt32(ORDER_STATUS.待处理))
                        {
                            order.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                            this._orderRepository.Update(order);
                        }

                        if (orderRow.Status == Convert.ToInt32(ORDER_STATUS.待处理))
                        {
                            orderRow.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                        }

                        orderRow.Sorting += sortingCount;
                        this._orderRowRepository.Update(orderRow);
                        OrderRowBatch orderRowBatch = new OrderRowBatch
                        {
                            Type = type,
                            CreateTime = DateTime.Now,
                            OrderId = orderId,
                            OrderRowId = orderId,
                            ReservoirAreaId = areaId,
                            BatchCount = sortingCount
                        };

                        this._orderRowBatchRepository.Add(orderRowBatch);
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                            LogDesc = string.Format("订单[{0}]出库,订单行[{1}],出库批次数量[{2}],出库方式[{3}]",
                                orderId,
                                orderRowId,
                                sortingCount,
                                Enum.GetName(typeof(ORDER_BATCH_TYPE),type)),
                            Founder = tag,
                            CreateTime = DateTime.Now
                        });
                        scope.Complete();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public async Task CloseOrder(int orderId, string tag)
        {
            Guard.Against.Zero(orderId,nameof(orderId));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                OrderSpecification orderSpec = new OrderSpecification(orderId,null,null,null,
                    null,null,null,null,null,null,
                    null,null,null,null,null,null);
                List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
                if(orders.Count==0)
                    throw new Exception(string.Format("订单Id[{0}],不存在！",orderId));

                Order order = orders[0];
                if(order.Status==Convert.ToInt32(ORDER_STATUS.执行中))
                    throw new Exception(string.Format("订单Id[{0}],正在执行中,无法关闭！",orderId));
                    

                OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, orderId,
                    null, null, null, null, null, null);
                List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        order.Status = Convert.ToInt32(ORDER_STATUS.关闭);
                        orderRows.ForEach(om => om.Status = Convert.ToInt32(ORDER_STATUS.关闭));
                        this._orderRepository.Update(order);
                        this._orderRowRepository.Update(orderRows);
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                            LogDesc = string.Format("关闭订单[{0}],关闭订单行[{1}]",orderId,string.Join(',',orderRows.ConvertAll(r=>r.Id))),
                            Founder = tag,
                            CreateTime = DateTime.Now
                        });
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public async Task CloseOrderRow(int orderRowId, string tag)
        {
            Guard.Against.Zero(orderRowId,nameof(orderRowId));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                OrderRowSpecification orderRowSpec = new OrderRowSpecification(orderRowId, null,
                    null, null, null, null, null, null);
                List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                if(orderRows.Count==0)
                    throw new Exception(string.Format("订单行d[{0}],不存在！",orderRowId));
                
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        orderRows[0].Status = Convert.ToInt32(ORDER_STATUS.关闭);
                        this._orderRowRepository.Update(orderRows[0]);
                        this._logRecordRepository.Add(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                            LogDesc = string.Format("关闭订单行[{0}]",orderRowId),
                            Founder = tag,
                            CreateTime = DateTime.Now
                        });
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

        }
    }
}
