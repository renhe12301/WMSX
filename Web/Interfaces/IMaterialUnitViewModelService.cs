using System;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IMaterialUnitViewModelService
    {
        Task<ResponseResultViewModel> AddMaterialUnit(MaterialUnitViewModel materialUnitViewModel);
        Task<ResponseResultViewModel> UpdateMaterialUnit(MaterialUnitViewModel materialUnitViewModel);
        Task<ResponseResultViewModel> DelMaterialUnit(MaterialUnitViewModel materialUnitViewModel);
        Task<ResponseResultViewModel> GetMaterialUnits(int? pageIndex, int? itemsPage,
                                                      int? id,string unitName);
    }
}
