using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrganizationManager;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class SysMenuService:ISysMenuService
    {
        private readonly IAsyncRepository<RoleMenu> _roleMenuRepository;
        public SysMenuService(IAsyncRepository<RoleMenu> roleMenuRepository)
        {
            this._roleMenuRepository = roleMenuRepository;
        }

        public async Task AssignMenu(int roleId, List<int> menuIds)
        {
            Guard.Against.Zero(roleId, nameof(roleId));
            Guard.Against.NullOrEmpty(menuIds, nameof(menuIds));
            List<RoleMenu> roleMenus=new List<RoleMenu>();
            menuIds.ForEach(async (menuId) =>
            {
                RoleMenu roleMenu = new RoleMenu
                {
                    SysRoleId=roleId,
                    SysMenuId= menuId
                };
                roleMenus.Add(roleMenu);
            });
            await this._roleMenuRepository.AddAsync(roleMenus);
        }
    }
}
