using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IWarehouseTrayViewModelService
    {
        Task<ResponseResultViewModel> GetTrays(int? pageIndex, int? itemsPage,
             int? id, string trayCode, string rangeMaterialCount,
            int? orderId, int? orderRowId, int? carrier,
            string trayTaskStatus, int? locationId,int? ouId, int? wareHouseId, int? areaId);
        
    }
}
