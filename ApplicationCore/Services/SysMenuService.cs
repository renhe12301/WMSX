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
        private readonly ITransactionRepository _transactionRepository;
        public SysMenuService(IAsyncRepository<RoleMenu> roleMenuRepository,
                              ITransactionRepository transactionRepository)
        {
            this._roleMenuRepository = roleMenuRepository;
            this._transactionRepository = transactionRepository;
        }

        public async Task AssignMenu(int roleId, List<int> menuIds)
        {
            Guard.Against.Zero(roleId, nameof(roleId));
            Guard.Against.NullOrEmpty(menuIds, nameof(menuIds));
            this._transactionRepository.Transaction( () =>
            {
                menuIds.ForEach(async (menuId) =>
                {
                    RoleMenu roleMenu = new RoleMenu
                    {
                        SysRoleId=roleId,
                        SysMenuId= menuId
                    };
                    await this._roleMenuRepository.AddAsync(roleMenu);
                });
            });
        }
    }
}
