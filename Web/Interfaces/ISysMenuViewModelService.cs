using System;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;

namespace Web.Interfaces
{
    public interface ISysMenuViewModelService
    {
        Task<ResponseResultViewModel> GetMenus(int roleId);
        Task<ResponseResultViewModel> GetMenuTrees(int rootId);
    }
}
