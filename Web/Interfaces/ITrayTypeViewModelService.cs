using System;
using Web.ViewModels;
using System.Threading.Tasks;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface ITrayTypeViewModelService
    {
        Task<ResponseResultViewModel> AddTrayType(TrayTypeViewModel trayTypeViewModel);
        Task<ResponseResultViewModel> UpdateTrayType(TrayTypeViewModel trayTypeViewModel);
        Task<ResponseResultViewModel> DelTrayType(TrayTypeViewModel trayTypeViewModel);
        Task<ResponseResultViewModel> AssignTrayDic(TrayTypeViewModel trayTypeViewModel);
        Task<ResponseResultViewModel> GetTrayTypes(int? pageIndex, int? itemsPage, int? id, int? parentId, string typeName);
        Task<ResponseResultViewModel> GetTrayTypeTree(int rootId);

    }
}
