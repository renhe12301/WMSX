using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IAsyncRepository<TrayDic> _trayDicRepository;
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly ITransactionRepository _transactionRepository;
        public OrderService(IAsyncRepository<Order> orderRepository,
                               IAsyncRepository<OrderRow> orderRowRepository,
                               IAsyncRepository<TrayDic> trayDicRepository,
                               IAsyncRepository<MaterialDic> materialDicRepository,
                               IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                               IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                               IAsyncRepository<ReservoirArea> reservoirRepository,
                               ITransactionRepository transactionRepository
                               )
        {
            this._orderRepository = orderRepository;
            this._orderRowRepository = orderRowRepository;
            this._trayDicRepository = trayDicRepository;
            this._materialDicRepository = materialDicRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._reservoirAreaRepository = reservoirRepository;
            this._transactionRepository = transactionRepository;
        }


        public async Task CreateOrder(Order order)
        {
            Guard.Against.Null(order, nameof(order));
            this._transactionRepository.Transaction(async () =>
            {
                Order addOrder = await this._orderRepository.AddAsync(order);
                order.OrderRow.ForEach(om => om.OrderId = addOrder.Id);
                await this._orderRowRepository.AddAsync(order.OrderRow);
            });
        }

        public async Task SortingOrder2Area(int orderId, int orderRowId, int sortingCount,
                                                   string trayCode, int areaId)
        {
            Guard.Against.Zero(orderId, nameof(orderId));
            Guard.Against.Zero(orderRowId, nameof(orderRowId));
            Guard.Against.Zero(sortingCount, nameof(sortingCount));
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));
            Guard.Against.Zero(areaId, nameof(areaId));
            if (trayCode.Split('#').Length < 2)
                throw new Exception("托盘编码不合法,无法分拣!");
            var trayDicSpec = new TrayDicSpecification(null, trayCode.Split('#')[0], null);
            var trayDics = await this._trayDicRepository.ListAsync(trayDicSpec);
            var trayDic = trayDics[0];
            var orderSpec = new OrderSpecification(orderId, null,null, null, null,
                null, null, null, null, null, null,null,null,null);
            var orders = await this._orderRepository.ListAsync(orderSpec);
           
            Guard.Against.NullOrEmpty(orders, nameof(orders));
            Order order = orders[0];
            var orderRowSpec = new OrderRowSpecification(orderRowId, null, null, null, null, null, null, null);
            var orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
            Guard.Against.NullOrEmpty(orderRows, nameof(orderRows));
            OrderRow orderRow = orderRows[0];
            var areaSpec = new ReservoirAreaSpecification(areaId,null, null, null,null, null);
            var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
            Guard.Against.NullOrEmpty(areas, nameof(areas));
            var area = areas[0];
            //验证物料字典是否存在
            var mcode = NPinyin.Pinyin.GetInitials(area.AreaName);
            var materialDicSpec = new MaterialDicSpecification(null, mcode,null,null);
            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
            MaterialDic materialDic = null;
            if (materialDics.Count == 0)
            {
                MaterialDic newMaterialDic = new MaterialDic
                {
                    MaterialName = area.AreaName,
                    Spec = area.AreaName,
                    MaterialCode = mcode
                };
                materialDic = await this._materialDicRepository.AddAsync(newMaterialDic);
            }
            else
            {
                materialDic = materialDics[0];
            }

            if (sortingCount > orderRow.PreCount) throw new Exception("分拣数量不能大于接收数量！");
            int surplusCount = orderRow.PreCount - orderRow.Sorting;
            if (sortingCount > surplusCount)
                throw new Exception(string.Format("已经分拣了{0}个,最多还能分拣{1}个", orderRow.Sorting,surplusCount));

            var warehouseTrayDetailSpec = new WarehouseTrayDetailSpecification(null,trayCode,
                                                       null,null,null, null, null,null, null,null, null,null,null);

            var whTrays =await this._warehouseTrayRepository.ListAsync(warehouseTrayDetailSpec);
            if (whTrays.Count > 0)
            {
                if (whTrays[0].Carrier.HasValue)
                    throw new Exception(string.Format("托盘已经绑定载体[{0}],无法分拣！",
                        whTrays[0].Carrier));
                else
                {
                    var whTray = whTrays[0];
                    var trayMaterials = whTray.WarehouseMaterial;
                    OrderRow preOrderRow = whTray.OrderRow;
                    preOrderRow.Sorting -= whTray.MaterialCount;
                    await this._orderRowRepository.UpdateAsync(preOrderRow);
                    await this._warehouseMaterialRepository.DeleteAsync(trayMaterials);
                    await this._warehouseTrayRepository.DeleteAsync(whTray);
                }
            }
                

            DateTime now = DateTime.Now;
            List<WarehouseMaterial> warehouseMaterials = whTrays[0].WarehouseMaterial;
            this._transactionRepository.Transaction(async () =>
            {
                orderRow.Sorting += sortingCount;
                await this._orderRowRepository.UpdateAsync(orderRow);
                if (whTrays.Count > 0)
                {
                    Task.WaitAll(this._warehouseTrayRepository.DeleteAsync(whTrays[0]),
                                 this._warehouseMaterialRepository.DeleteAsync(warehouseMaterials));
                }
                WarehouseTray warehouseTray = new WarehouseTray
                {
                    Code = trayCode,
                    CreateTime = now,
                    OrderId = orderId,
                    OrderRowId = orderRowId,
                    MaterialCount = sortingCount,
                    TrayDicId = trayDic.Id,
                    TrayStep = Convert.ToInt32(TRAY_STEP.待入库)
                };
                var addTray = await this._warehouseTrayRepository.AddAsync(warehouseTray);
                WarehouseMaterial warehouseMaterial = new WarehouseMaterial
                {
                    OrderId = orderId,
                    OrderRowId = orderRowId,
                    CreateTime = now,
                    WarehouseTrayId = addTray.Id,
                    MaterialCount = sortingCount,
                    MaterialDicId= materialDic.Id
                };
                await this._warehouseMaterialRepository.AddAsync(warehouseMaterial);
            });
        }

    }
}
