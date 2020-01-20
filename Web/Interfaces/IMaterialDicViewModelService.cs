using System;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IMaterialDicViewModelService
    {
       Task<ResponseResultViewModel> AddMaterialDic(MaterialDicViewModel  materialDicViewModel);
       Task<ResponseResultViewModel> UpdateMaterialDic(MaterialDicViewModel materialDicViewModel);
       Task<ResponseResultViewModel> DelMaterialDic(MaterialDicViewModel materialDicViewModel);
       Task<ResponseResultViewModel> GetMaterialDics(int? pageIndex, int? itemsPage,
                                                      int? id, string materialCode,
                                                      string materialName,string spec,
                                                      int? typeId);
    }
}
