using System;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface ISysMenuViewModelService
    {
        Task<ResponseResultViewModel> GetMenus(int roleId);
        Task<ResponseResultViewModel> GetMenuTrees(int rootId);
    }
}
