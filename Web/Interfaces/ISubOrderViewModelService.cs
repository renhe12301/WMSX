using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.OrderManager;

namespace Web.Interfaces
{
    public interface ISubOrderViewModelService
    {
        Task<ResponseResultViewModel> CreateOrder(SubOrderViewModel subOrderViewModel);

        Task<ResponseResultViewModel> ScrapOrder(SubOrderViewModel subOrderViewModel);

        Task<ResponseResultViewModel> ScrapOrderRow(List<SubOrderRowViewModel> subOrderRowViewModels);
    }
}