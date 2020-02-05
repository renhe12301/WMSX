using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.StockManager;

namespace ApplicationCore.Interfaces
{
    public interface IInOutTaskService
    {
        Task EmptyAwaitOutApply(List<WarehouseTray> warehouseTrays);
        Task EmptyAwaitInApply(string trayCode,int orgId);
        Task InApply(string trayCode, string locationCode);
        Task AwaitOutApply(int orderId,int orderRowId,List<WarehouseTray> warehouseTrays);
        Task TaskStepReport(int id,int vid, int taskStep);
    }
}
