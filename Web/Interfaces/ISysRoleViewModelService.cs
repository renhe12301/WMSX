﻿using System;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.AuthorityManager;

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
          int? id,string roleName);
    }
}
