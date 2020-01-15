using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Interfaces
{
    public interface IRoleService
    {
        Task AddRole(SysRole role);
        Task UpdateRole(int id, string roleName);
        Task Logout(List<int> roleIds);
        Task Enable(List<int> roleIds);
        Task AssignMenu(int roleId, List<int> menuIds);
    }
}
