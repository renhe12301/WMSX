using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IWarehouseViewModelService
    {
        Task<ResponseResultViewModel> AddWarehouse(WarehouseViewModel warehouseViewModel);
        Task<ResponseResultViewModel> UpdateWarehouse(WarehouseViewModel warehouseViewModel);
        Task<ResponseResultViewModel> Disable(WarehouseViewModel warehouseViewModel);
        Task<ResponseResultViewModel> Enable(WarehouseViewModel warehouseViewModel);
        Task<ResponseResultViewModel> GetWarehouses(int? pageIndex, int? itemsPage,
                                      int? id,int? orgId, string whName);
    }
}
