using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Entities.TaskManager;

namespace ApplicationCore.Interfaces
{
    public interface IInOutTaskService
    {
        Task EmptyOut(int pyId,double outCount);
        Task EmptyEntry(string trayCode,int pyId);

        Task TrayEntry(string trayCode, int pyId);

        Task EntryApply(string fromPort,string barCode,int cargoHeight,string cargoWeight);
        
        Task TaskReport(int taskId, long reportTime, int taskStatus, string error);

        Task OutConfirm(string trayCode);
    }
}
