using System;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IInOutRecordViewModelService
    {
        public Task<ResponseResultViewModel> GetInOutRecords(int? pageIndex,
                                             int? itemsPage,int? type,
                                             string sCreateTime, string eCreateTime);

    }
}
