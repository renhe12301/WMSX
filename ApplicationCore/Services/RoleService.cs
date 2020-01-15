using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;
using ApplicationCore.Misc;


namespace ApplicationCore.Services
{
    public class RoleService:IRoleService
    {
        private IAsyncRepository<SysRole> _sysRoleRepository;
        private IAsyncRepository<EmployeeRole> _employeeRoleRepository;
        private IAsyncRepository<RoleMenu> _roleMenuRepository;
        public RoleService(IAsyncRepository<SysRole> sysRoleRepository,
                            IAsyncRepository<EmployeeRole> employeeRoleRepository,
                            IAsyncRepository<RoleMenu> roleMenuRepository)
        {
            this._sysRoleRepository = sysRoleRepository;
            this._employeeRoleRepository = employeeRoleRepository;
            this._roleMenuRepository = roleMenuRepository;
        }

        public async Task AddRole(SysRole role)
        {
            Guard.Against.Null(role, nameof(role));
            Guard.Against.NullOrEmpty(role.RoleName, nameof(role.RoleName));
            SysRoleSpecification roleSpec = new SysRoleSpecification(null, role.RoleName);
            var roles = await this._sysRoleRepository.ListAsync(roleSpec);
            if (roles.Count > 0) throw new Exception(string.Format("角色名称[{0}],已经存在！", role.RoleName));
            await this._sysRoleRepository.AddAsync(role);
        }


        public async Task AssignMenu(int roleId, List<int> menuIds)
        {
            Guard.Against.Zero(roleId, nameof(roleId));
            Guard.Against.NullOrEmpty(menuIds, nameof(menuIds));
            RoleMenuSpecification rmSpec=new RoleMenuSpecification(roleId);
            var rms = await this._roleMenuRepository.ListAsync(rmSpec);
            if (rms.Count > 0) await this._roleMenuRepository.DeleteAsync(rms);
            List<RoleMenu> addRoleMenus=new List<RoleMenu>();
            menuIds.ForEach(async (mId) =>
            {
                RoleMenu roleMenu = new RoleMenu();
                roleMenu.SysMenuId = mId;
                roleMenu.SysRoleId = roleId;
                addRoleMenus.Add(roleMenu);
            });
            await this._roleMenuRepository.AddAsync(addRoleMenus);
        }

        public async Task Enable(List<int> roleIds)
        {
            Guard.Against.NullOrEmpty(roleIds, nameof(roleIds));
            var roles = await this._sysRoleRepository.ListAllAsync();
            List<SysRole> updRoles=new List<SysRole>();
            roles.ForEach(role =>
            {
                if (roleIds.Contains(role.Id))
                {
                    role.Status = Convert.ToInt32(SYSROLE_STATUS.正常);
                    updRoles.Add(role);
                }
            });
            if (updRoles.Count > 0)
                await this._sysRoleRepository.UpdateAsync(updRoles);
        }

        public async Task Logout(List<int> roleIds)
        {
            Guard.Against.NullOrEmpty(roleIds, nameof(roleIds));
            var roles = await this._sysRoleRepository.ListAllAsync();
            List<SysRole> updRoles=new List<SysRole>();
            roles.ForEach(role =>
            {
                if (roleIds.Contains(role.Id))
                {
                    role.Status = Convert.ToInt32(SYSROLE_STATUS.注销);
                    updRoles.Add(role);
                }
            });
            if (updRoles.Count > 0)
                await this._sysRoleRepository.UpdateAsync(updRoles);
        }

        public async Task UpdateRole(int id, string roleName)
        {
            Guard.Against.Null(roleName, nameof(roleName));
            Guard.Against.Zero(id, nameof(id));
            SysRoleSpecification roleSpec = new SysRoleSpecification(null, roleName);
            var roles = await this._sysRoleRepository.ListAsync(roleSpec);
            if (roles.Count > 0) throw new Exception(string.Format("角色名称[{0}],已经存在！", roleName));
            roleSpec = new SysRoleSpecification(id, null);
            roles = await this._sysRoleRepository.ListAsync(roleSpec);
            if (roles.Count == 0) throw new Exception(string.Format("角色编号[{0}],不存在",id));
            var role = roles[0];
            role.RoleName = roleName;
            await this._sysRoleRepository.UpdateAsync(role);
        }
    }
}
