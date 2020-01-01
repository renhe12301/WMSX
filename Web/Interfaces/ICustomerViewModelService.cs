using System;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface ICustomerViewModelService
    {
        Task<ResponseResultViewModel> AddCustomer(CustomerViewModel customerViewModel);
        Task<ResponseResultViewModel> GetCustomers(int? pageIndex,int? itemsPage,int? id,string customerName);
    }
}
