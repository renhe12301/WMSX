using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.TaskManager;
using Web.ViewModels.StockManager;
namespace Web.Interfaces
{
    public interface IInOutTaskViewModelService
    {

        Task<ResponseResultViewModel> EmptyOut(WarehouseTrayViewModel warehouseTrayViewModel);

        Task<ResponseResultViewModel> EmptyEntry(WarehouseTrayViewModel warehouseTrayViewModel);
        
        Task<ResponseResultViewModel> TrayEntry(WarehouseTrayViewModel warehouseTrayViewModel);

        Task<ResponseResultViewModel> EntryApply(WarehouseTrayViewModel warehouseTrayViewModel);
        
        Task<ResponseResultViewModel> TaskReport(InOutTaskViewModel inOutTaskViewModel);
        Task<ResponseResultViewModel> GetInOutTasks(int? pageIndex,int? itemsPage,int? id,string trayCode,string materialCode,
                                                    int? subOrderId,int? subOrderRowId,
                                                    string status,string steps,string types,int? ouId,
                                                    int? wareHouseId, int? areaId,int? pyId,
                                                    string sCreateTime,string eCreateTIme,
                                                    string sFinishTime,string eFinishTime);
        
        Task<ResponseResultViewModel> OutConfirm(WarehouseTrayViewModel warehouseTrayViewModel);
        
        
    }
}
