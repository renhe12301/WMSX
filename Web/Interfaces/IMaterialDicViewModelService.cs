using System;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IMaterialDicViewModelService
    {
        Task<ResponseResultViewModel> GetMaterialDics(int? pageIndex, int? itemsPage,
                                                      int? id, string materialCode,
                                                      string materialName,string spec,
                                                      int? typeId);
    }
}
