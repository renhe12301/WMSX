using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;
using ApplicationCore.Misc;
using System.Collections.Generic;
using System.Dynamic;

namespace Web.Services
{
    public class SysRoleViewModelService:ISysRoleViewModelService
    {
        private readonly IRoleService _roleService;
        private readonly IAsyncRepository<SysRole> _sysRoleRepository;
        private readonly IAsyncRepository<EmployeeRole> _employeeRoleRoleRepository;

        public SysRoleViewModelService(IRoleService roleService,
                                        IAsyncRepository<SysRole> sysRoleRepository,
                                        IAsyncRepository<EmployeeRole> employeeRoleRoleRepository)
        {
            this._roleService = roleService;
            this._sysRoleRepository = sysRoleRepository;
            this._employeeRoleRoleRepository = employeeRoleRoleRepository;
        }

        public async Task<ResponseResultViewModel> AddRole(SysRoleViewModel sysRoleViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                SysRole sysRole = new SysRole
                {
                    CreateTime = DateTime.Now,
                    RoleName = sysRoleViewModel.RoleName,
                    ParentId=sysRoleViewModel.ParentId
                };
               await this._roleService.AddRole(sysRole);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> AssignMenu(RoleMenuViewModel roleMenuViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._roleService.AssignMenu(roleMenuViewModel.RoleId, roleMenuViewModel.MenuIds);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Enable(SysRoleViewModel sysRoleViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await _roleService.Enable(sysRoleViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetRoles(int? pageIndex, int? itemsPage, int? id,int? parentId, string roleName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<SysRole> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SysRolePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,parentId,roleName);
                }
                else
                {
                    baseSpecification = new SysRoleSpecification(id,parentId, roleName);
                }
                var roles = await this._sysRoleRepository.ListAsync(baseSpecification);
                List<SysRoleViewModel> roleViewModels = new List<SysRoleViewModel>();
                roles.ForEach(e =>
                {
                    SysRoleViewModel roleViewModel = new SysRoleViewModel
                    {
                        RoleName = e.RoleName,
                        CreateTime = e.CreateTime.ToString(),
                        Status = Enum.GetName(typeof(SYSROLE_STATUS), e.Status),
                        Id = e.Id
                    };
                    roleViewModels.Add(roleViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._sysRoleRepository.CountAsync(new SysRoleSpecification(id,parentId,roleName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = roleViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = roleViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetRoleTrees(int rootId, string depthTag)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var roleSpec = new SysRoleSpecification(rootId,null, null);
                var roles = await this._sysRoleRepository.ListAsync(roleSpec);
                if (roles.Count == 0) throw new Exception(string.Format("角色编号{0}不存在", rootId));
                var role = roles[0];
                TreeViewModel current = new TreeViewModel
                {
                    Id = role.Id,
                    ParentId = role.ParentId,
                    Name = role.RoleName,
                    Type = "dir"
                };
                await RoleTree(current, current.Children);
                response.Data = current;

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        private async Task RoleTree(TreeViewModel current, List<TreeViewModel> childs)
        {
            var roleSpec = new SysRoleSpecification(null, current.Id, null);
            var roles = await this._sysRoleRepository.ListAsync(roleSpec);
            roles.ForEach(async (role) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = role.Id,
                    ParentId = current.Id,
                    Name = role.RoleName
                };

                if (role.Type == 1)
                {
                    child.Type = "dir";
                    childs.Add(child);
                    await RoleTree(child, child.Children);
                }
                else
                {
                    child.Type = "role";
                    childs.Add(child);
                }
            });
        }


        public async Task<ResponseResultViewModel> Logout(SysRoleViewModel sysRoleViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await _roleService.Logout(sysRoleViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> UpdateRole(SysRoleViewModel sysRoleViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await _roleService.UpdateRole(sysRoleViewModel.Id,sysRoleViewModel.RoleName);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }
    }
}
