using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IMaterialTypeViewModelService
    {
        Task<ResponseResultViewModel> AddMaterialType(MaterialTypeViewModel materialTypeViewModel);
        Task<ResponseResultViewModel> UpdateMaterialType(MaterialTypeViewModel materialTypeViewModel);
        Task<ResponseResultViewModel> DelMaterialType(MaterialTypeViewModel materialTypeViewModel);
        Task<ResponseResultViewModel> GetMaterialTypes(int? pageIndex, int? itemsPage,int? id,int? parentId,string typeName);
        Task<ResponseResultViewModel> GetMaterialTypeTrees(int rootId);
        Task<ResponseResultViewModel> AssignMaterialDic(MaterialTypeViewModel materialTypeViewModel);
    }
}
