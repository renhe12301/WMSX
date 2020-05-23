using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.OrderManager;

namespace Web.Interfaces
{
    public interface ISubOrderViewModelService
    {
        
        Task<ResponseResultViewModel> GetOrders(int? pageIndex, int? itemsPage,
            int?id,string orderNumber, int? orderTypeId, string status,int? ouId,int? warehouseId,
            int? pyId,int? supplierId, string supplierName,int? supplierSiteId,string supplierSiteName,
            string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime);
        
        
        Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, int? id, int? subOrderId,int? orderRowId,
            int? orderTypeId,int? ouId,int? warehouseId,int? pyId,int? supplierId, string supplierName,int? supplierSiteId,
            string supplierSiteName,string status,string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime);
        
        Task<ResponseResultViewModel> CreateOrder(SubOrderViewModel subOrderViewModel);

        Task<ResponseResultViewModel> ScrapOrder(SubOrderViewModel subOrderViewModel);

        Task<ResponseResultViewModel> ScrapOrderRow(List<SubOrderRowViewModel> subOrderRowViewModels);
    }
}