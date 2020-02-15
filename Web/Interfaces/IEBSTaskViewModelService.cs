using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IEBSTaskViewModelService
    {
        Task<ResponseResultViewModel> GetTasks(int? pageIndex, int? itemsPage,int? id,string taskName,int? projectId, 
            string sCreateTime, string eCreateTime,
            string sEndTime, string eEndTime);
    }
}