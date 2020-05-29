using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;
using Web.ViewModels;
using Web.ViewModels.OrderManager;
namespace Web.Interfaces
{
    public interface IOrderViewModelService
    {

        Task<ResponseResultViewModel> GetOrders(int? pageIndex, int? itemsPage,
             int?id,string orderNumber,int? sourceId, int? orderTypeId, string status,int? ouId,int? warehouseId,int? pyId, string applyUserCode, string approveUserCode,
             int? employeeId,string employeeName, int? supplierId, string supplierName, string sApplyTime, string eApplyTime, string sApproveTime,
             string eApproveTime, string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime);
        
        Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, int? id, int? orderId,int? sourceId, string status,
            string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime);
        
        Task<ResponseResultViewModel> GetTKOrderMaterials(int ouId,int warehouseId,int? areaId);

    }

}
