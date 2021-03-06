using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.OrderManager;

namespace Web.Interfaces
{
    public interface ISubOrderViewModelService
    {
        
        Task<ResponseResultViewModel> GetOrders(int? pageIndex, int? itemsPage,
            string ids,string orderNumber,int? sourceId, string orderTypeIds,string businessType, string status,int? isBack,int? ouId,int? warehouseId,
             int? pyId,int? supplierId, string supplierName,int? supplierSiteId,string supplierSiteName,
            string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime);
        
        
        Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, string ids, int? subOrderId,int? orderRowId,int? sourceId,
            string orderTypeIds,int? ouId,int? warehouseId, int? reservoirAreaId,string businessType, string ownerType, int? pyId,int? supplierId, string supplierName,int? supplierSiteId,
            string supplierSiteName,string status,string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime);

        Task<ResponseResultViewModel> SortingNoneOrder(string materialCode, double sortingCount, string trayCode, int areaId, int pyId);

        Task<ResponseResultViewModel> SortingOrder(int subOrderId, int subOrderRowId, double sortingCount, string trayCode,int areaId, int pyId);
        
        Task<ResponseResultViewModel> CreateOrder(SubOrderViewModel subOrderViewModel);

        Task<ResponseResultViewModel> CreateTKOrder(SubOrderViewModel subOrderViewModel);

        Task<ResponseResultViewModel> ScrapOrder(SubOrderViewModel subOrderViewModel);

        Task<ResponseResultViewModel> ScrapOrderRow(List<SubOrderRowViewModel> subOrderRowViewModels);
        
        Task<ResponseResultViewModel> OutConfirm(int subOrderId,int pyId);

    }
}