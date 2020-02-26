using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface ILogRecordViewModelService
    {
        Task<ResponseResultViewModel> GetLogRecords(int? pageIndex, int? itemsPage,string logTypes,string logDesc,
                                                   string founder,string sCreateTime,string eCreateTIme);
    }
}