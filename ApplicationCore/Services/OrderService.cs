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


        public async Task CreateOutOrder(Order order)
        {
            Guard.Against.Null(order, nameof(order));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    OrderSpecification orderSpec = new OrderSpecification(order.Id,null,null, null, null,
                        null,null,null,null,null, null, null,
                        null, null, null, null,
                        null, null, null, null, null);
                    List<Order> orders = await this._orderRepository.ListAsync(orderSpec);

                    OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, order.Id,null,null,
                        null, null, null, null, null, null);
                    List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);

                    //对占用订单行里面物料数量进行校验
                    OrderRowSpecification checkOrderRowSpec = new OrderRowSpecification(null, null,null,null, null,
                        new List<int> {Convert.ToInt32(ORDER_STATUS.待处理), Convert.ToInt32(ORDER_STATUS.执行中)}, null,
                        null, null, null);
                    List<OrderRow> checkOrderRows = await this._orderRowRepository.ListAsync(checkOrderRowSpec);
                    List<OrderRow> tkOrderRows = checkOrderRows.Where(or =>
                        or.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库) &&
                        or.Order.OUId == order.OUId && or.Order.WarehouseId == order.WarehouseId).ToList();

                    WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                        null,
                        null, null, null, null, null, null, null,
                        null, new List<int>() {Convert.ToInt32(TRAY_STEP.入库完成), Convert.ToInt32(TRAY_STEP.初始化)},
                        null, order.OUId, order.WarehouseId, null, null, null,null,null,null);
                    List<WarehouseMaterial> allWarehouseMaterials =
                        await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);

                    if (orders.Count > 0)
                    {
                        var srcOrder = orders[0];
                        if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.完成))
                            throw new Exception(string.Format("订单[{0}]已经完成无法修改！", srcOrder.OrderNumber));

                        if (srcOrder.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                            throw new Exception(string.Format("订单[{0}]已经关闭无法修改！", srcOrder.OrderNumber));

                        List<OrderRow> addOrderRows = new List<OrderRow>();
                        List<OrderRow> updOrderRows = new List<OrderRow>();
                        
                        order.OrderRow.ForEach((eor) =>
                        {
                            var existRow = orderRows.Find(r => r.RowNumber == eor.RowNumber);
                            if (existRow == null)
                            {
                                List<WarehouseMaterial> warehouseMaterials = allWarehouseMaterials.Where(w =>
                                        w.MaterialDicId == eor.MaterialDicId &&
                                        w.ReservoirAreaId == eor.ReservoirAreaId)
                                    .ToList();
                                int stockCount = warehouseMaterials.Sum(m => m.MaterialCount);
                                int occCount = tkOrderRows.Where(or => or.ReservoirAreaId == eor.ReservoirAreaId &&
                                                                       or.MaterialDicId == eor.MaterialDicId)
                                    .Sum(or => or.PreCount);

                                int remainingCount = stockCount - occCount;
                                if (remainingCount < eor.PreCount)
                                    throw new Exception(string.Format("新增订单行物料Id[{0}],库存不足,出库失败！\n", eor.MaterialDicId));

                                OrderRow addOrderRow = new OrderRow
                                {
                                    OrderId = srcOrder.Id,
                                    ReservoirAreaId = eor.ReservoirAreaId,
                                    RowNumber = eor.RowNumber,
                                    MaterialDicId = eor.MaterialDicId,
                                    PreCount = Convert.ToInt32(eor.PreCount),
                                    Price = Convert.ToInt32(eor.Price),
                                    Amount = eor.PreCount * eor.Price
                                };
                                addOrderRows.Add(addOrderRow);
                            }
                            else
                            {
                                if (existRow.Status == Convert.ToInt32(ORDER_STATUS.完成))
                                    throw new Exception(string.Format("订单行[{0}]已经完成无法修改！\n", srcOrder.OrderNumber));

                                if (existRow.Status == Convert.ToInt32(ORDER_STATUS.关闭))
                                    throw new Exception(string.Format("订单行[{0}]已经关闭无法修改！\n", srcOrder.OrderNumber));

                                if (Convert.ToInt32(eor.PreCount) > (existRow.PreCount - existRow.Sorting))
                                {
                                    int re = Convert.ToInt32(eor.PreCount) -
                                             (existRow.PreCount - existRow.Sorting.GetValueOrDefault());
                                    List<WarehouseMaterial> warehouseMaterials = allWarehouseMaterials.Where(w =>
                                            w.MaterialDicId == eor.MaterialDicId &&
                                            w.ReservoirAreaId == eor.ReservoirAreaId)
                                        .ToList();
                                    int stockCount = warehouseMaterials.Sum(m => m.MaterialCount);
                                    int occCount = tkOrderRows.Where(or =>
                                            or.ReservoirAreaId == existRow.ReservoirAreaId &&
                                            or.MaterialDicId == eor.MaterialDicId)
                                        .Sum(or => or.PreCount);

                                    int remainingCount = stockCount - occCount;
                                    if (remainingCount < re)
                                        throw new Exception(string.Format("订单行Id[{0}],物料Id[{1}],库存不足,出库失败！\n", existRow.Id,
                                            eor.MaterialDicId));
                                    else
                                    {
                                        existRow.PreCount = Convert.ToInt32(eor.PreCount);
                                        existRow.Amount = Convert.ToInt32(eor.PreCount) * eor.Price;
                                        updOrderRows.Add(existRow);
                                    }
                                }
                                else
                                {
                                    existRow.PreCount = Convert.ToInt32(eor.PreCount);
                                    existRow.Amount = Convert.ToInt32(eor.PreCount) * eor.Price;
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
                    else
                    {
                        if (allWarehouseMaterials.Count > 0)
                        {
                            order.OrderRow.ForEach((o) =>
                            {
                                List<WarehouseMaterial> warehouseMaterials = allWarehouseMaterials.Where(w =>
                                        w.MaterialDicId == o.MaterialDicId && w.ReservoirAreaId == o.ReservoirAreaId)
                                    .ToList();
                                int stockCount = warehouseMaterials.Sum(m => m.MaterialCount);
                                int occCount = tkOrderRows.Where(or => or.ReservoirAreaId == o.ReservoirAreaId &&
                                                                       or.MaterialDicId == o.MaterialDicId)
                                    .Sum(or => or.PreCount);
                                int remainingCount = stockCount - occCount;
                                if (remainingCount < o.PreCount)
                                    throw new Exception(string.Format("物料Id[{0}],物料名称[{1}],库存不足,出库失败！\n", o.MaterialDicId,
                                        o.MaterialDic.MaterialName));
                                o.Amount = o.PreCount * o.Price;
                            });
                            
                        }

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
        
        public async Task OrderOut(int orderId, int orderRowId, int areaId, int sortingCount, int type, string tag)
        {
            Guard.Against.Zero(orderId, nameof(orderId));
            Guard.Against.Zero(orderRowId, nameof(orderRowId));
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.Zero(sortingCount, nameof(sortingCount));
    
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    var orderSpec = new OrderSpecification(orderId, null, null, null,null,null, null,
                        null,null, null, null, null, null, null, null,
                        null, null, null, null, null, null);
                    var orders = await this._orderRepository.ListAsync(orderSpec);
                    if (orders.Count == 0) throw new Exception(string.Format("订单编号[{0}],不存在！", orderId));
                    Order order = orders[0];
                    var orderRowSpec = new OrderRowSpecification(orderRowId, null,null,null,
                        null, null, null, null, null, null);
                    var orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                    if (orderRows.Count == 0) throw new Exception(string.Format("订单行编号[{0}],不存在！", orderRowId));
                    OrderRow orderRow = orderRows[0];
                    var areaSpec = new ReservoirAreaSpecification(areaId, null, null, null, null, null);
                    var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
                    if (areas.Count == 0) throw new Exception(string.Format("子库存编号[{0}],不存在！", areaId));
                    var area = areas[0];

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
                                    Enum.GetName(typeof(ORDER_BATCH_TYPE), type)),
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
                catch (Exception ex)
                {
                    await this._logRecordRepository.AddAsync(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw ex;
                }
            }
            
        }

        public async Task CloseOrder(int orderId, string tag)
        {
            Guard.Against.Zero(orderId, nameof(orderId));

            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    OrderSpecification orderSpec = new OrderSpecification(orderId, null, null, null,
                        null,null,null,null, null, null, null, null, null,
                        null, null, null, null, null, null, null, null);
                    List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
                    if (orders.Count == 0)
                        throw new Exception(string.Format("订单Id[{0}],不存在！", orderId));

                    Order order = orders[0];
                    if (order.Status == Convert.ToInt32(ORDER_STATUS.执行中))
                        throw new Exception(string.Format("订单Id[{0}],正在执行中,无法关闭！", orderId));


                    OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, orderId,null,null,
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
                                LogDesc = string.Format("关闭订单[{0}],关闭订单行[{1}]", orderId,
                                    string.Join(',', orderRows.ConvertAll(r => r.Id))),
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
                catch (Exception ex)
                {
                    await this._logRecordRepository.AddAsync(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw ex;
                }
            }
            
        }

        public async Task CloseOrderRow(int orderRowId, string tag)
        {
            Guard.Against.Zero(orderRowId, nameof(orderRowId));
            StringBuilder errorSB = new StringBuilder();
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    OrderRowSpecification orderRowSpec = new OrderRowSpecification(orderRowId, null,null,null,
                        null, null, null, null, null, null);
                    List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                    if (orderRows.Count == 0)
                        throw new Exception(string.Format("订单行d[{0}],不存在！", orderRowId));

                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        try
                        {
                            orderRows[0].Status = Convert.ToInt32(ORDER_STATUS.关闭);
                            this._orderRowRepository.Update(orderRows[0]);
                            this._logRecordRepository.Add(new LogRecord
                            {
                                LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                                LogDesc = string.Format("关闭订单行[{0}]", orderRowId),
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
                catch (Exception ex)
                {
                    await this._logRecordRepository.AddAsync(new LogRecord
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
}
