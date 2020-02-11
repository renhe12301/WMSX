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
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Entities.SysManager;

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
        public OrderService(IAsyncRepository<Order> orderRepository,
                               IAsyncRepository<OrderRow> orderRowRepository,
                               IAsyncRepository<MaterialDic> materialDicRepository,
                               IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                               IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                               IAsyncRepository<ReservoirArea> reservoirRepository
                               )
        {
            this._orderRepository = orderRepository;
            this._orderRowRepository = orderRowRepository;
            this._materialDicRepository = materialDicRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._reservoirAreaRepository = reservoirRepository;
        }


        public async Task CreateOrder(Order order)
        {
            Guard.Against.Null(order, nameof(order));
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Order addOrder = this._orderRepository.Add(order);
                    order.OrderRow.ForEach(om => om.OrderId = addOrder.Id);
                    this._orderRowRepository.Add(order.OrderRow);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
           
        }

        public async Task SortingOrder2Area(int orderId, int orderRowId, int sortingCount,string trayCode)
        {
            Guard.Against.Zero(orderId, nameof(orderId));
            Guard.Against.Zero(orderRowId, nameof(orderRowId));
            Guard.Against.Zero(sortingCount, nameof(sortingCount));
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));
            if (trayCode.Split('#').Length < 2)
                throw new Exception("托盘编码不合法,无法分拣!");
            var orderSpec = new OrderSpecification(orderId, null,null, null, null,
                null, null, null, null, null, null,null,null,null);
            var orders = await this._orderRepository.ListAsync(orderSpec);
           
            Guard.Against.NullOrEmpty(orders, nameof(orders));
            Order order = orders[0];
            var orderRowSpec = new OrderRowSpecification(orderRowId, null, null, null, null, null, null, null);
            var orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
            Guard.Against.NullOrEmpty(orderRows, nameof(orderRows));
            OrderRow orderRow = orderRows[0];
            var areaSpec = new ReservoirAreaSpecification(orderRow.ReservoirAreaId,null, null, null,null,null);
            var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
            Guard.Against.NullOrEmpty(areas, nameof(areas));
            var area = areas[0];
            var materialDicSpec = new MaterialDicSpecification(orderRow.MaterialDicId, null,null,null,null);
            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
            if (materialDics.Count == 0)
                throw new Exception(string.Format("物料字典[{0}]),不存在！",orderRow.MaterialDicId));
            
            MaterialDic materialDic = materialDics[0];
            
            if (sortingCount > orderRow.PreCount) throw new Exception("分拣数量不能大于接收数量！");
            int surplusCount = orderRow.PreCount - orderRow.Sorting;
            if (sortingCount > surplusCount)
                throw new Exception(string.Format("已经分拣了{0}个,最多还能分拣{1}个", orderRow.Sorting,surplusCount));

            var warehouseTrayDetailSpec = new WarehouseTrayDetailSpecification(null,trayCode,
                                                       null,null,null, null, null,null, null,null, null);
            
            var whTrays =await this._warehouseTrayRepository.ListAsync(warehouseTrayDetailSpec);
            if (whTrays.Count > 0)
            {
                if (whTrays[0].TrayStep!=Convert.ToInt32(TRAY_STEP.初始化))
                    throw new Exception("托盘未初始化,无法分拣！");
                var whTray = whTrays[0];
                var trayMaterials = whTray.WarehouseMaterial;
                if(trayMaterials.Count==0)
                    throw  new Exception("当前托盘不是空托盘,无法分拣！");
            }

            DateTime now = DateTime.Now;
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    orderRow.Sorting += sortingCount;
                    this._orderRowRepository.Update(orderRow);
                    if (whTrays.Count == 0)
                    {
                        WarehouseTray warehouseTray = new WarehouseTray
                        {
                            TrayCode = trayCode,
                            CreateTime = now,
                            OrderId = orderId,
                            OrderRowId = orderRowId,
                            MaterialCount = sortingCount,
                            TrayStep = Convert.ToInt32(TRAY_STEP.待入库)
                        };
                        var addTray = this._warehouseTrayRepository.Add(warehouseTray);
                        WarehouseMaterial warehouseMaterial = new WarehouseMaterial
                        {
                            OrderId = orderId,
                            OrderRowId = orderRowId,
                            CreateTime = now,
                            WarehouseTrayId = addTray.Id,
                            MaterialCount = sortingCount,
                            MaterialDicId = materialDic.Id
                        };
                        this._warehouseMaterialRepository.Add(warehouseMaterial);
                    }
                    else
                    {
                        WarehouseTray warehouseTray = whTrays[0];
                        warehouseTray.MaterialCount = 0;
                        warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.待入库);
                        warehouseTray.CreateTime = DateTime.Now;;
                        warehouseTray.OrderId = orderId;
                        warehouseTray.OrderRowId = orderRowId;
                        this._warehouseTrayRepository.Update(warehouseTray);
                        WarehouseMaterial warehouseMaterial = new WarehouseMaterial
                        {
                            OrderId = orderId,
                            OrderRowId = orderRowId,
                            CreateTime = now,
                            WarehouseTrayId = warehouseTray.Id,
                            MaterialCount = sortingCount,
                            MaterialDicId = materialDic.Id
                        };
                        this._warehouseMaterialRepository.Add(warehouseMaterial);
                    }

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
