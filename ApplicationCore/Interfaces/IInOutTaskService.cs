using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.StockManager;

namespace ApplicationCore.Interfaces
{
    public interface IInOutTaskService
    {
        Task EmptyOut(int areaId,int outCount);
        Task EmptyEntry(string trayCode,int areaId);

        Task EntryApply(string fromPort,string barCode,int cargoHeight,string cargoWeight);
        
        Task TaskReport(int taskId, long reportTime, int taskStatus, string error);
    }
}
