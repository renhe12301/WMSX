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
    public class OrderRowBatchViewModelService:IOrderRowBatchViewModelService
    {
        private readonly IAsyncRepository<OrderRowBatch> _orderRowBatchRepository;

        public OrderRowBatchViewModelService(IAsyncRepository<OrderRowBatch> orderRowBatchRepository)
        {
            this._orderRowBatchRepository = orderRowBatchRepository;
        }

        public async Task<ResponseResultViewModel> GetOrderRowBatchs(int? pageIndex, int? itemsPage, int? id, int? orderId,
            int? orderRowId, int? areaId, int? isRead,int? type, int? isSync, string status)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<OrderRowBatch> spec = null;
                List<int> orderStatuss = null;
                if (!string.IsNullOrEmpty(status))
                {
                    orderStatuss = status.Split(new char[]{','}, 
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    spec = new OrderRowBatchPaginatedSpecification(pageIndex.Value,itemsPage.Value,id,orderId,
                        orderRowId,areaId,isRead,type,isSync,orderStatuss);
                }
                else
                {
                    spec = new OrderRowBatchSpecification(id,orderId, orderRowId,areaId,isRead,type,isSync,orderStatuss);
                }
                var orderRowBatchs = await this._orderRowBatchRepository.ListAsync(spec);
                List<OrderRowBatchViewModel> orderRowBatchViewModels = new List<OrderRowBatchViewModel>();
                
                orderRowBatchs.ForEach(e =>
                {
                    OrderRowBatchViewModel orderRowBatchViewModel = new OrderRowBatchViewModel
                    {
                          Id = e.Id,
                          BatchCount = e.BatchCount,
                          RealityCount = e.RealityCount.GetValueOrDefault(),
                          TypeStr = Enum.GetName(typeof(ORDER_BATCH_TYPE),e.Type),
                          StatusStr = Enum.GetName(typeof(ORDER_STATUS),e.Status),
                          IsReadStr = Enum.GetName(typeof(ORDER_BATCH_READ),e.IsRead),
                          IsSyncStr = Enum.GetName(typeof(ORDER_BATCH_SYNC),e.IsSync)
                    };
                    orderRowBatchViewModels.Add(orderRowBatchViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._orderRowBatchRepository.CountAsync(new OrderRowBatchSpecification(id,orderId,
                        orderRowId,areaId,isRead,type,isSync,orderStatuss));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = count;
                    dyn.total = orderRowBatchViewModels;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = orderRowBatchViewModels;
                }
                
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