using System;
using System.Collections.Generic;
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
using ApplicationCore.Entities.SysManager;
using ApplicationCore.Entities.TaskManager;

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
        public OrderService(IAsyncRepository<Order> orderRepository,
                               IAsyncRepository<OrderRow> orderRowRepository,
                               IAsyncRepository<MaterialDic> materialDicRepository,
                               IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                               IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                               IAsyncRepository<ReservoirArea> reservoirRepository,
                               IAsyncRepository<InOutRecord> inOutRecordRepository,
                               IAsyncRepository<OrderRowBatch> orderRowBatchRepository
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
        }


        public async Task<int> CreateOrder(Order order)
        {
            Guard.Against.Null(order, nameof(order));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {

                }

                catch (Exception ex)
                {
                    
                }

                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        this._orderRepository.Add(order);
                        order.OrderRow.ForEach(om => om.OrderId = order.Id);
                        this._orderRowRepository.Add(order.OrderRow);
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return order.Id;
            }
        }

        public async Task SortingOrder(int orderId, int orderRowId, int sortingCount,int badCount,string trayCode,int areaId)
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
                    null, null, null, null, null, null, null);

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
                                OUId = area.OUId
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
                            InOutCount = warehouseTray.MaterialCount + warehouseTray.OutCount,
                            IsRead = 0,
                            CreateTime = DateTime.Now,
                            Type = 0,
                            Status = Convert.ToInt32(TASK_STATUS.待处理)
                        };
                        this._inOutTaskRepository.Add(inOutRecord);

                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

        }

        public async Task OrderOut(int orderId, int orderRowId, int areaId, int sortingCount,int type)
        {
            Guard.Against.Zero(orderId,nameof(orderId));
            Guard.Against.Zero(orderRowId,nameof(orderRowId));
            Guard.Against.Zero(areaId,nameof(areaId));
            Guard.Against.Zero(sortingCount,nameof(sortingCount));
            
            var orderSpec = new OrderSpecification(orderId, null,null, null, null,
                null, null,null,null, null, null, null, 
                null,null,null,null);
            var orders = await this._orderRepository.ListAsync(orderSpec);
            if(orders.Count==0)throw new Exception(string.Format("订单编号[{0}],不存在！",orderId));
            Order order = orders[0];
            var orderRowSpec = new OrderRowSpecification(orderRowId, null,null, null, null, null, null, null);
            var orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
            if(orderRows.Count==0)throw new Exception(string.Format("订单行编号[{0}],不存在！",orderRowId));
            OrderRow orderRow = orderRows[0];
            var areaSpec = new ReservoirAreaSpecification(areaId,null, null, null,null,null);
            var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
            if(areas.Count==0)throw new Exception(string.Format("子库存编号[{0}],不存在！",areaId));
            var area = areas[0];
            
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                if ((orderRow.PreCount-orderRow.CancelCount - orderRow.Sorting) > sortingCount)
                    throw new Exception(string.Format("出库数量[{0}]大于订单行申请数量[{1}]！",
                        sortingCount,orderRow.PreCount-orderRow.CancelCount));
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
