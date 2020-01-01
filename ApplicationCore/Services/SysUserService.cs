using System;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrganizationManager;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using ApplicationCore.Misc;

namespace ApplicationCore.Services
{
    public class SysUserService:ISysUserService
    {
        private IAsyncRepository<SysUser> _sysUserRepository;

        public SysUserService(IAsyncRepository<SysUser> sysUserRepository)
        {
            this._sysUserRepository = sysUserRepository;
        }

        public async Task AddUser(SysUser user)
        {
            Guard.Against.Null(user,nameof(user));
            await this._sysUserRepository.AddAsync(user);
        }

        public async Task Enable(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            SysUserSpecification userSpec = new SysUserSpecification(id, null,null);
            var users = await this._sysUserRepository.ListAsync(userSpec);
            Guard.Against.Zero(users.Count,nameof(users));
            var suser = users[0];
            suser.Status=Convert.ToInt32(SYSUSER_STATUS.正常);
            await this._sysUserRepository.UpdateAsync(suser);
        }

        public async Task Logout(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            SysUserSpecification userSpec = new SysUserSpecification(id, null,null);
            var users = await this._sysUserRepository.ListAsync(userSpec);
            Guard.Against.Zero(users.Count, nameof(users));
            var suser = users[0];
            suser.Status = Convert.ToInt32(SYSUSER_STATUS.注销);
            await this._sysUserRepository.UpdateAsync(suser);
        }

        public async Task UpdateUser(int id,string loginName)
        {
            Guard.Against.Zero(id, nameof(id));
            SysUserSpecification userSpec = new SysUserSpecification(null, null, loginName);
            var users = await this._sysUserRepository.ListAsync(userSpec);
            if (users.Count > 0) throw new Exception(string.Format("登录名{0}已经存在！",loginName));
            userSpec = new SysUserSpecification(id, null, null);
            users = await this._sysUserRepository.ListAsync(userSpec);
            var suser = users[0];
            suser.LoginName = loginName;
            await this._sysUserRepository.UpdateAsync(suser);
        }
    }
}
