using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.BasicInformation;

using ApplicationCore.Specifications;
using ApplicationCore.Misc;
using System.Linq;

namespace Web.Services
{
    public class InOrderViewModelService:IOrderViewModelService
    {

        private readonly IOrderService _orderService;
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<ReservoirArea> _areaRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;

        public InOrderViewModelService(IOrderService orderService,
                                       IAsyncRepository<Order> orderRepository,
                                       IAsyncRepository<ReservoirArea> areaRepository,
                                       IAsyncRepository<Warehouse> warehouseRepository)
        {
            this._orderService = orderService;
            this._orderRepository = orderRepository;
            this._areaRepository = areaRepository;
            this._warehouseRepository = warehouseRepository;
        }

        async Task<ResponseResultViewModel> GetOrderList(int includeDetail,BaseSpecification<Order> spec)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var orders = await this._orderRepository.ListAsync(spec);
                List<OrderViewModel> orderViewModels = new List<OrderViewModel>();
                orders.ForEach(order =>
                {
                    OrderViewModel ovm = new OrderViewModel
                    {
                        Id = order.Id,
                        OrderNumber = order.OrderNumber,
                        OrderType = Enum.GetName(typeof(ORDER_TYPE), order.OrderTypeId),
                        ApplyUserCode = order.ApplyUserCode,
                        ApproveUserCode = order.ApproveUserCode,
                        ApplyTime = order.ApplyTime.ToString(),
                        ApproveTime = order.ApproveTime.ToString(),
                        CallingParty = order.CallingParty,
                        CreateTime=order.CreateTime.ToString(),
                        FinishTime=order.FinishTime.ToString(),
                        Progress = order.Progress
                    };
                    if (includeDetail > 0)
                    {
                        List<OrderRowViewModel> orderRowViewModels = new List<OrderRowViewModel>();
                        order.OrderRow.ForEach(om =>
                        {
                            var orderRowViewModel = new OrderRowViewModel
                            {
                                OrderId = order.Id,
                                PreCount = om.PreCount,
                                CreateTime = om.CreateTime.ToString(),
                                FinishTime = om.FinishTime.ToString(),
                                Progress = om.Progress,
                                RealityCount=om.RealityCount,
                                ReservoirAreaName=om.ReservoirArea.AreaName,
                                Sorting = om.Sorting,
                                Id = om.Id

                            };
                            orderRowViewModels.Add(orderRowViewModel);
                        });
                        ovm.OrderRows = orderRowViewModels;
                    }
                    orderViewModels.Add(ovm);
                });
                response.Data = orderViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

      

        public async Task<ResponseResultViewModel> GetOrders(int? pageIndex,int? itemsPage,
            int? includeDetail,string orderNumber,
            int? orderTypeId,string progressRange, string applyUserCode, string approveUserCode,
            string sApplyTime, string eApplyTime, string sApproveTime,
            string eApproveTime,string sCreateTime,string eCreateTime,string sFinishTime,string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Order> spec = null;
                List<int> orderProgress = null;
                if (!string.IsNullOrEmpty(progressRange))
                {
                    orderProgress = progressRange.Split(new char[]{
               ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (includeDetail.HasValue&&includeDetail > 0)
                {
                    if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                    {
                        spec = new OrderPaginatedDetailSpecification(pageIndex.Value,itemsPage.Value,null, orderNumber, orderTypeId,
                        orderProgress, applyUserCode, approveUserCode, sApplyTime, eApplyTime, sApproveTime, eApproveTime,
                          sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                    }
                    else
                    {
                        spec = new OrderDetailSpecification(null, orderNumber, orderTypeId,
                        orderProgress, applyUserCode, approveUserCode, sApplyTime, eApplyTime, sApproveTime, eApproveTime,
                         sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                    }
                   
                }
                else
                {
                    if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                    {
                        spec = new OrderPaginatedSpecification(pageIndex.Value, itemsPage.Value, null, orderNumber, orderTypeId,
                        orderProgress, applyUserCode, approveUserCode, sApplyTime, eApplyTime, sApproveTime, eApproveTime,
                         sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                    }
                    else
                    {
                        spec = new OrderSpecification(null, orderNumber, orderTypeId,
                        orderProgress, applyUserCode, approveUserCode, sApplyTime, eApplyTime, sApproveTime, eApproveTime,
                        sCreateTime,eCreateTime,sFinishTime,eFinishTime);
                    }
                }

                var result = await GetOrderList(includeDetail.HasValue?includeDetail.Value:0,spec);
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> CreateOrder(OrderViewModel orderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                Order order = new Order
                {
                    OrderNumber = orderViewModel.OrderNumber,
                    CreateTime = DateTime.Now,
                    ApplyTime = DateTime.Parse(orderViewModel.ApplyTime),
                    ApplyUserCode = orderViewModel.ApplyUserCode,
                    ApproveTime = DateTime.Parse(orderViewModel.ApproveTime),
                    ApproveUserCode = orderViewModel.ApproveUserCode,
                    CallingParty = orderViewModel.CallingParty,
                    OrderTypeId = orderViewModel.OrderTypeId
                };
                List<OrderRow> orderRows = new List<OrderRow>();
              
                orderViewModel.OrderRows.ForEach(async(or) =>
                {
                    ReservoirArea area = null;
                    if (or.ReservoirAreaId.HasValue)
                    {
                        ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(or.ReservoirAreaId,null,null,null, null, null);
                        var areas = await this._areaRepository.ListAsync(reservoirAreaSpec);
                        if (areas.Count == 0) throw new Exception(string.Format("子库区[{0}]不存在！",or.ReservoirAreaId));
                        area = areas[0];
                    }
                    WarehouseSpecification warehouseSpec = new WarehouseSpecification(or.WarehouseId, null,null);
                    var warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                    if (warehouses.Count == 0) throw new Exception(string.Format("库组织[{0}],不存在！", or.WarehouseId));
                    var warehouse = warehouses[0];
                    OrderRow orderRow = new OrderRow
                    {
                        CreateTime=DateTime.Now,
                        PreCount=or.PreCount,
                        RealityCount=or.PreCount
                    };
                    if (area != null)
                        orderRow.ReservoirAreaId = area.Id;
                });
               await this._orderService.CreateOrder(order);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResultViewModel> SortingOrder2Area(OrderRowViewModel orderRow)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._orderService.SortingOrder2Area(orderRow.OrderId,
                    orderRow.Id, orderRow.Sorting, orderRow.TrayCode);
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
