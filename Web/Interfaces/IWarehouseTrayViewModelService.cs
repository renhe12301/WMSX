using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IWarehouseTrayViewModelService
    {
        Task<ResponseResultViewModel> GetTrays(int? pageIdex, int? itemsPage,
            int? includeDetail, int? id, string trayCode, string rangeMaterialCount,
            int? trayDicId, int? orderId, int? orderRowId, int? carrier,
            string trayTaskStatus, int? locationId,int? orgId,int? ouId, int? wareHouseId, int? areaId);

    }
}
