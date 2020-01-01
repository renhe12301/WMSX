using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Interfaces
{
    public interface ISysUserService
    {
        Task AddUser(SysUser user);
        Task UpdateUser(int id,string loginName);
        Task Logout(int id);
        Task Enable(int id);
    }
}
