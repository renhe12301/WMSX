using System;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;

namespace Web.Interfaces
{
    public interface ISysUserViewModelService
    {
        Task<ResponseResultViewModel> AddUser(SysUserViewModel userViewModel);
        Task<ResponseResultViewModel> UpdateUser(SysUserViewModel userViewModel);
        Task<ResponseResultViewModel> Enable(SysUserViewModel userViewModel);
        Task<ResponseResultViewModel> Logout(SysUserViewModel userViewModel);
        Task<ResponseResultViewModel> GetUsers(int? pageIndex, int? itemsPage,
            int? id, int? employeeId, string loginName);
    }
}
