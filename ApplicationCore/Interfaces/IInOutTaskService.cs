using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IInOutTaskService
    {
        Task EmptyAwaitInApply(string trayCode,int warehouseId,int areaId);
        Task InApply(string trayCode, string locationCode);
        Task AwaitOutApply(int orderId,int orderRowId,List<string> trayCodes);
        Task TaskStepReport(int id,int vid,string vName, int taskStep);
    }
}
