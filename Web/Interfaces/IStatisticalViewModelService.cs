using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IStatisticalViewModelService
    {
        Task<ResponseResultViewModel> MaterialChart(int ouId);
        
        Task<ResponseResultViewModel> MaterialSheet(int ouId);
        
        Task<ResponseResultViewModel> InRecordChart(int ouId, int queryType);
        
        Task<ResponseResultViewModel> InRecordSheet(int ouId, int queryType);
        
        Task<ResponseResultViewModel> OutRecordChart(int ouId, int queryType);
        
        Task<ResponseResultViewModel> OutRecordSheet(int ouId, int queryType);
        
        Task<ResponseResultViewModel> InSubOrderChart(int ouId, int queryType);
        
        Task<ResponseResultViewModel> InSubOrderSheet(int ouId, int queryType);
        
        Task<ResponseResultViewModel> OutSubOrderChart(int ouId, int queryType);
        Task<ResponseResultViewModel> OutSubOrderSheet(int ouId, int queryType);
        
        
        Task<ResponseResultViewModel> InOrderChart(int ouId, int queryType);
        Task<ResponseResultViewModel> InOrderSheet(int ouId, int queryType);
        
        Task<ResponseResultViewModel> OutOrderChart(int ouId, int queryType);
        Task<ResponseResultViewModel> OutOrderSheet(int ouId, int queryType);
        
    }
}