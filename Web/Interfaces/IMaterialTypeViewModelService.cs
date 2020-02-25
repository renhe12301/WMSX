using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IMaterialTypeViewModelService
    {
        Task<ResponseResultViewModel> GetMaterialTypes(int? pageIndex, int? itemsPage,int? id,int? parentId,string typeName);
        Task<ResponseResultViewModel> GetMaterialTypeDics(int? pageIndex, int? itemsPage,int? typeId);
        Task<ResponseResultViewModel> GetMaterialTypeTrees(int rootId);
        Task<ResponseResultViewModel> MaterialTypeChart(int ouId);
    }
}
