using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.OrderManager;

namespace Web.Services
{
    public class SubOrderViewModelService:ISubOrderViewModelService
    {

        private IAsyncRepository<SubOrder> _subOrderRepository;
        private IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        private ISubOrderService _subOrderService;
        
        public SubOrderViewModelService(IAsyncRepository<SubOrder> subOrderRepository,
            ISubOrderService subOrderService,
            IAsyncRepository<SubOrderRow> subOrderRowRepository
            )
        {
            this._subOrderRepository = subOrderRepository;
            this._subOrderService = subOrderService;
            this._subOrderRowRepository = subOrderRowRepository;
        }

        public async Task<ResponseResultViewModel> GetOrders(int? pageIndex, int? itemsPage, int? id, string orderNumber, 
            int? sourceId,string orderTypeIds, string status, int? ouId,int? warehouseId,int? pyId, int? supplierId, string supplierName,
            int? supplierSiteId,string supplierSiteName,string sCreateTime, string eCreateTime, string sFinishTime,string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<SubOrder> baseSpecification = null;
                List<int> orderStatuss = null;
                if (!string.IsNullOrEmpty(status))
                {
                    orderStatuss = status.Split(new char[]{','}, 
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                List<int> orderTypes = null;
                if (!string.IsNullOrEmpty(orderTypeIds))
                {
                    orderTypes = orderTypeIds.Split(new char[]{','}, 
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SubOrderPaginatedSpecification(pageIndex.Value, itemsPage.Value,id,orderNumber,sourceId,
                        orderTypes,orderStatuss,null,null,ouId,warehouseId,pyId,
                        supplierId,supplierName,supplierSiteId,supplierSiteName,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new SubOrderSpecification(id, orderNumber,sourceId, orderTypes, null, null,
                        null, ouId, warehouseId, pyId, supplierId, supplierName, supplierSiteId, supplierSiteName,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }

                var orders = await this._subOrderRepository.ListAsync(baseSpecification);
                List<SubOrderViewModel> orderViewModels = new List<SubOrderViewModel>();
                orders.ForEach(e =>
                {
                    SubOrderViewModel subOrderViewModel = new SubOrderViewModel
                    {
                        Id = e.Id,
                        OrderNumber = e.OrderNumber,
                        OrderTypeId = e.OrderTypeId,
                        OrderType = e.OrderType?.TypeName,
                        Progress = e.Progress,
                        FinishTime = e.FinishTime?.ToString(),
                        CreateTime = e.CreateTime?.ToString(),
                        WarehouseId = e.WarehouseId,
                        WarehouseName = e.Warehouse?.WhName,
                        OUId = e.OUId,
                        OUName = e.OU?.OUName,
                        SupplierId = e.SupplierId,
                        SupplierName = e.Supplier?.SupplierName,
                        SupplierSiteId = e.SupplierSiteId,
                        SupplierSiteName = e.SupplierSite?.SiteName,
                        Currency = e.Currency,
                        TotalAmount = e.TotalAmount,
                        BusinessTypeCode = e.BusinessTypeCode,
                        Status = e.Status,
                        StatusStr = Enum.GetName(typeof(ORDER_STATUS), e.Status),
                        PhyWarehouseId = e.PhyWarehouseId,
                        PhyWarehouseName = e.PhyWarehouse?.PhyName,
                        IsScrap = e.IsScrap,
                        SourceId = e.SourceId,
                        SourceOrderType = e.SourceOrderType,
                        IsSyncStr = e.IsSync==1?"已同步":"未同步"
                    };
                    orderViewModels.Add(subOrderViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._subOrderRepository.CountAsync(new SubOrderSpecification(id,orderNumber,sourceId,orderTypes,orderStatuss,null,null,
                        ouId,warehouseId,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,sCreateTime,eCreateTime,sFinishTime,eFinishTime));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = orderViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = orderViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, int? id, int? subOrderId, int? orderRowId,int? sourceId, string orderTypeIds,
            int? ouId, int? warehouseId, int? pyId, int? supplierId, string supplierName, int? supplierSiteId,
            string supplierSiteName, string status, string sCreateTime, string eCreateTime, string sFinishTime,
            string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<SubOrderRow> baseSpecification = null;
                List<int> orderStatuss = null;
                if (!string.IsNullOrEmpty(status))
                {
                    orderStatuss = status.Split(new char[]{','}, 
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                List<int> orderTypes = null;
                if (!string.IsNullOrEmpty(orderTypeIds))
                {
                    orderTypes = orderTypeIds.Split(new char[]{','}, 
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SubOrderRowPaginatedSpecification(pageIndex.Value,itemsPage.Value,id,subOrderId,orderRowId,sourceId,
                        orderTypes,ouId,warehouseId,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new SubOrderRowSpecification(id,subOrderId,orderRowId,sourceId,
                        orderTypes,ouId,warehouseId,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime);
                }
                var orderRows = await this._subOrderRowRepository.ListAsync(baseSpecification);
                List<SubOrderRowViewModel> orderRowViewModels = new List<SubOrderRowViewModel>();
                orderRows.ForEach(e =>
                {
                    SubOrderRowViewModel subOrderRowViewModel = new SubOrderRowViewModel
                    {
                        Id = e.Id,
                        SubOrderId = e.SubOrderId,
                        RowNumber = e.RowNumber,
                        ReservoirAreaId = e.ReservoirAreaId,
                        ReservoirAreaName = e.ReservoirArea?.AreaName,
                        MaterialDicId = e.MaterialDicId,
                        MaterialDicName = e.MaterialDic?.MaterialName,
                        CreateTime = e.CreateTime.ToString(),
                        FinishTime = e.FinishTime.ToString(),
                        PreCount = e.PreCount,
                        Sorting = e.Sorting,
                        RealityCount = e.RealityCount,
                        Progress = e.Progress,
                        Status = e.Status,
                        StatusStr = Enum.GetName(typeof(ORDER_STATUS),e.Status),
                        OrderRowId = e.OrderRowId,
                        Price = e.Price,
                        Amount = e.Amount,
                        UseFor = e.UseFor,
                        IsScrap = e.IsScrap,
                        OUId = e.SubOrder.OUId,
                        WarehouseId = e.SubOrder.WarehouseId,
                        SupplierId = e.SubOrder.SupplierId,
                        SupplierSiteId = e.SubOrder.SupplierSiteId,
                        SourceId = e.SourceId,
                        OrderTypeId = e.SubOrder?.OrderTypeId
                    };
                    orderRowViewModels.Add(subOrderRowViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._subOrderRowRepository.CountAsync(new SubOrderRowSpecification(id,subOrderId,orderRowId,sourceId,
                        orderTypes,ouId,warehouseId,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = orderRowViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = orderRowViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> SortingOrder(int subOrderId, int subOrderRowId, double sortingCount, string trayCode, int areaId, string tag)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._subOrderService.SortingOrder(subOrderId, subOrderRowId, sortingCount, trayCode, areaId, tag);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResultViewModel> CreateOrder(SubOrderViewModel subOrderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                DateTime now = DateTime.Now;
                SubOrder order = new SubOrder
                {
                    OUId = subOrderViewModel.OUId,
                    WarehouseId = subOrderViewModel.WarehouseId,
                    OrderNumber = subOrderViewModel.OrderNumber??"O"+now.Ticks,
                    CreateTime = now,
                    OrderTypeId = subOrderViewModel.OrderTypeId,
                    Status = 0,
                    SupplierId = subOrderViewModel.SupplierId,
                    SupplierSiteId = subOrderViewModel.SupplierSiteId,
                    Currency = subOrderViewModel.Currency,
                    BusinessTypeCode = subOrderViewModel.BusinessTypeCode,
                    TotalAmount = subOrderViewModel.TotalAmount,
                    SourceId = subOrderViewModel.SourceId,
                    SourceOrderType = subOrderViewModel.SourceOrderType
                    
                };
                List<SubOrderRow> subOrderRows = new List<SubOrderRow>();
                subOrderViewModel.SubOrderRows.ForEach(async(or) =>
                {
                    SubOrderRow orderRow = new SubOrderRow
                    {
                        CreateTime=now,
                        PreCount=or.PreCount,
                        ReservoirAreaId = or.ReservoirAreaId,
                        MaterialDicId = or.MaterialDicId,
                        Price = or.Price,
                        Status = 0,
                        OrderRowId = or.OrderRowId,
                        RowNumber = or.RowNumber??"OR"+now.Ticks,
                        UseFor = or.UseFor,
                        Amount = or.Amount,
                        SourceId = or.SourceId
                        
                    };
                    subOrderRows.Add(orderRow);
                });
                order.SubOrderRow = subOrderRows;
                await  this._subOrderService.CreateOrder(order);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> ScrapOrder(SubOrderViewModel subOrderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                SubOrder order = new SubOrder
                {
                    Id = subOrderViewModel.Id
                };
                await  this._subOrderService.ScrapOrder(order);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> ScrapOrderRow(List<SubOrderRowViewModel> subOrderRowViewModels)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                List<SubOrderRow> orderRows = new List<SubOrderRow>();
                subOrderRowViewModels.ForEach(async(or) =>
                {
                    SubOrderRow orderRow = new SubOrderRow
                    {
                        Id=or.Id,
                        OrderRowId = or.OrderRowId
                    };
                    orderRows.Add(orderRow);
                });
                await  this._subOrderService.ScrapOrderRow(orderRows);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> OutConfirm(int subOrderId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await  this._subOrderService.OutConfirm(subOrderId);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }
    }
}