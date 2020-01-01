using System;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface ITrayDicViewModelService
    {
        Task<ResponseResultViewModel> AddTrayDic(TrayDicViewModel trayDicViewModel);
        Task<ResponseResultViewModel> UpdateTrayDic(TrayDicViewModel trayDicViewModel);
        Task<ResponseResultViewModel> DelTrayDic(TrayDicViewModel trayDicViewModel);
        Task<ResponseResultViewModel> GetTrayDics(int? pageIndex, int? itemsPage,
                                                      int? id, string trayCode,
                                                      string trayName,
                                                      int? typeId);
    }
}
