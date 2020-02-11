using System;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface ISupplierViewModelService
    {
        Task<ResponseResultViewModel> GetSuppliers(int ?pageIndex, int ?itemsPage,int? id, string supplierName);
    }
}
