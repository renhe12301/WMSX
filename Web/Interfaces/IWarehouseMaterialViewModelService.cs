using System;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IWarehouseMaterialViewModelService
    {
        Task<ResponseResultViewModel> GetMaterials(int? pageIndex,
            int? itemsPage, int? id, string materialCode, int? materialDicId, string trayCode,
            int? trayDicId, int? orderId, int? orderRowId, int? carrier, string trayTaskStatus, int? locationId,
            int? orgId, int? ouId, int? wareHouseId, int? areaId);
    }
}
