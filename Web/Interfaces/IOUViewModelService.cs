using System;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IOUViewModelService
    {
        Task<ResponseResultViewModel> GetOUs(int? pageIndex, int? itemsPage, int? id,string ouName,string ouCode,string companyName);
        
        Task<ResponseResultViewModel> GetOUTrees(int rootId,string depthTag);
    }
}
