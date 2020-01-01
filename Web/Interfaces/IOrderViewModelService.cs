using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;
using Web.ViewModels;
using Web.ViewModels.OrderManager;
namespace Web.Interfaces
{
    public interface IOrderViewModelService
    {

        Task<ResponseResultViewModel> GetOrders(int? pageIndex, int? itemsPage,
            int? includeDetail, string orderNumber,
            int? orderTypeId, string progressRange, string applyUserCode, string approveUserCode,
            string sApplyTime, string eApplyTime, string sApproveTime,
            string eApproveTime, string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime);

        Task<ResponseResultViewModel> CreateOrder(OrderViewModel order);
        Task<ResponseResultViewModel> SortingOrder2Area(OrderRowViewModel orderRow);

    }

}
