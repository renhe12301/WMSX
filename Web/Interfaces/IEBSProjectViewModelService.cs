using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IEBSProjectViewModelService
    {
        Task<ResponseResultViewModel> GetProjects(int? pageIndex, int? itemsPage,
            int? id,string projectName,int? ouId, 
            string sCreateTime, string eCreateTime,
            string sEndTime, string eEndTime);
    }
}