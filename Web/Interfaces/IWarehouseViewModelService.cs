using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IWarehouseViewModelService
    {
        Task<ResponseResultViewModel> GetWarehouses(int? pageIndex, int? itemsPage,
                                      int? id,int? ouId, string whName);
    }
}
