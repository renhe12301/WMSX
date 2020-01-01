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
       Task<ResponseResultViewModel> EmptyAwaitInApply(WarehouseTrayViewModel warehouseTrayViewModel);
       Task<ResponseResultViewModel> InApply(InOutTaskViewModel inOutTaskViewModel);
       Task<ResponseResultViewModel> AwaitOutApply(InOutTaskViewModel inOutTaskViewModel);
       Task<ResponseResultViewModel> TaskStepReport(InOutTaskViewModel inOutTaskViewModel);
        Task<ResponseResultViewModel> GetInOutTasks(int? pageIndex,int? itemsPage,int? id,
                                                   string status,string steps,string sCreateTime,string eCreateTIme,
                                                    string sFinishTime,string eFinishTime);
    }
}
