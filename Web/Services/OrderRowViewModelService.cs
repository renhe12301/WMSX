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
    public class OrderRowViewModelService:IOrderRowViewModelService
    {

        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        
        public OrderRowViewModelService(IAsyncRepository<OrderRow> orderRowRepository)
        {
            this._orderRowRepository = orderRowRepository;
        }

        public async Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, int? id, int? orderId, string status,
                                               string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<OrderRow> spec = null;
                List<int> orderStatuss = null;
                if (!string.IsNullOrEmpty(status))
                {
                    orderStatuss = status.Split(new char[]{','}, 
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    spec = new OrderRowPaginatedSpecification(pageIndex.Value,itemsPage.Value,id,orderId,null,null,orderStatuss,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }
                else
                {
                    spec = new OrderRowSpecification(id,orderId,null,null,null,orderStatuss,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }
                var orderRows = await this._orderRowRepository.ListAsync(spec);
                List<OrderRowViewModel> orderRowViewModels = new List<OrderRowViewModel>();
                
                orderRows.ForEach(e =>
                {
                    OrderRowViewModel orderRowViewModel = new OrderRowViewModel
                    {
                        Id = e.Id,
                        RowNumber = e.RowNumber,
                        ReservoirAreaId = e.ReservoirAreaId,
                        ReservoirAreaName = e.ReservoirArea?.AreaName,
                        MaterialDicId = e.MaterialDicId,
                        MaterialDicName = e.MaterialDic?.MaterialName,
                        CreateTime = e.CreateTime.ToString(),
                        FinishTime = e.FinishTime?.ToString(),
                        PreCount = e.PreCount,
                        RealityCount = e.RealityCount.GetValueOrDefault(),
                        Sorting = e.Sorting.GetValueOrDefault(),
                        Status = e.Status,
                        StatusStr = Enum.GetName(typeof(ORDER_STATUS), e.Status),
                        Progress = e.Progress.GetValueOrDefault(),
                        EBSTaskName = e.EBSTask?.TaskName,
                        Price = e.Price,
                        Amount = e.Amount,
                        OrderId = e.OrderId,
                        RelatedId = e.RelatedId

                    };
                    orderRowViewModels.Add(orderRowViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._orderRowRepository.CountAsync(new OrderRowSpecification(id,orderId,null,null,null
                        ,orderStatuss,sCreateTime, eCreateTime, sFinishTime, eFinishTime));
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
    }
}