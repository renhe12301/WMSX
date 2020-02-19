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
             int?id,string orderNumber, int? orderTypeId, string status, string applyUserCode, string approveUserCode,
             int? employeeId,string employeeName,string sApplyTime, string eApplyTime, string sApproveTime,
             string eApproveTime, string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime);
        
        
        Task<ResponseResultViewModel> CreateOrder(OrderViewModel orderViewModel);
        
        Task<ResponseResultViewModel> SortingOrder(OrderRowViewModel orderRow);

    }

}
