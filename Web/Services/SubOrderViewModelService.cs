using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Interfaces;
using Web.Interfaces;
using Web.ViewModels;
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
                List<SubOrderRow> orderRows = new List<SubOrderRow>();
                orderRows.ForEach(async(or) =>
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
                    orderRows.Add(orderRow);
                });
                order.SubOrderRow = orderRows;
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
                orderRows.ForEach(async(or) =>
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