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
using Web.ViewModels.BasicInformation;
using Web.ViewModels.OrderManager;

namespace Web.Services
{
    public class SubOrderViewModelService:ISubOrderViewModelService
    {

        private IAsyncRepository<SubOrder> _subOrderRepository;
        private ISubOrderService _subOrderService;
        
        public SubOrderViewModelService(IAsyncRepository<SubOrder> subOrderRepository,
            ISubOrderService subOrderService)
        {
            this._subOrderRepository = subOrderRepository;
            this._subOrderService = subOrderService;
        }

        public async Task<ResponseResultViewModel> GetOrders(int? pageIndex, int? itemsPage, int? id, string orderNumber, 
            int? orderTypeId, string status, int? ouId,int? warehouseId,int? pyId, int? supplierId, string supplierName,
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
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SubOrderPaginatedSpecification(pageIndex.Value, itemsPage.Value,id,orderNumber,
                        orderTypeId,orderStatuss,ouId,warehouseId,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new SubOrderSpecification(id,orderNumber,orderTypeId,null,ouId,warehouseId,pyId,
                        supplierId,supplierName,supplierSiteId,supplierSiteName,sCreateTime,eCreateTime,sFinishTime,eFinishTime);
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
                        IsScrap = e.IsScrap
                    };
                    orderViewModels.Add(subOrderViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._subOrderRepository.CountAsync(new SubOrderSpecification(id,orderNumber,orderTypeId,orderStatuss,ouId,warehouseId,pyId,
                        supplierId,supplierName,supplierSiteId,supplierSiteName,sCreateTime,eCreateTime,sFinishTime,eFinishTime));
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

        public Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, int? id, int? subOrderId, int? orderRowId, int? orderTypeId,
            int? ouId, int? warehouseId, int? pyId, int? supplierId, string supplierName, int? supplierSiteId,
            string supplierSiteName, string status, string sCreateTime, string eCreateTime, string sFinishTime,
            string eFinishTime)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseResultViewModel> SortingOrder(int subOrderId, int subOrderRowId, int sortingCount, string trayCode, int areaId, string tag)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                this._subOrderService.SortingOrder(subOrderId, subOrderRowId, sortingCount, trayCode, areaId, tag);
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
                    OrderNumber = subOrderViewModel.OrderNumber??"Sub_Order_"+now.Ticks,
                    CreateTime = now,
                    OrderTypeId = subOrderViewModel.OrderTypeId,
                    Status = 0,
                    SupplierId = subOrderViewModel.SupplierId,
                    SupplierSiteId = subOrderViewModel.SupplierSiteId
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
                        OrderRowId = or.OrderRowId,
                        RowNumber = or.RowNumber??"Sub_Order_Row_"+now.Ticks,
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
    }
}