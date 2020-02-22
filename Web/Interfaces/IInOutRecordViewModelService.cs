using System;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IInOutRecordViewModelService
    {
        public Task<ResponseResultViewModel> GetInOutRecords(int? pageIndex,
                                             int? itemsPage,string trayCode,string materialName,int? type,int? ouId,
                                             int? wareHouseId, int? areaId,int? orderId,int? orderRowId,string status,
                                             string sCreateTime, string eCreateTime);

    }
}
