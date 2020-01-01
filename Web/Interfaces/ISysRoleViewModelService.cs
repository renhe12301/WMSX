using System;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrganizationManager;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;

namespace Web.Interfaces
{
    public interface ISysRoleViewModelService
    {
        Task<ResponseResultViewModel> AddRole(SysRoleViewModel sysRoleViewModel);
        Task<ResponseResultViewModel> UpdateRole(SysRoleViewModel sysRoleViewModel);
        Task<ResponseResultViewModel> Logout(SysRoleViewModel sysRoleViewModel);
        Task<ResponseResultViewModel> Enable(SysRoleViewModel sysRoleViewModel);
        Task<ResponseResultViewModel> AssignMenu(RoleMenuViewModel roleMenuViewModel);
        Task<ResponseResultViewModel> GetRoles(int? pageIndex, int? itemsPage,
          int? id,int? parentId,string roleName);
        Task<ResponseResultViewModel> GetRoleTrees(int rootId, string depthTag);
    }
}
