using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.AuthorityManager;
using ApplicationCore.Entities.FlowRecord;
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
        private IAsyncRepository<LogRecord> _logRecordRepository;
        public RoleService(IAsyncRepository<SysRole> sysRoleRepository,
                            IAsyncRepository<EmployeeRole> employeeRoleRepository,
                            IAsyncRepository<RoleMenu> roleMenuRepository,
                            IAsyncRepository<LogRecord> logRecordRepository)
        {
            this._sysRoleRepository = sysRoleRepository;
            this._employeeRoleRepository = employeeRoleRepository;
            this._roleMenuRepository = roleMenuRepository;
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddRole(SysRole role)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(role, nameof(role));
                    Guard.Against.NullOrEmpty(role.RoleName, nameof(role.RoleName));
                    SysRoleSpecification roleSpec = new SysRoleSpecification(null, role.RoleName);
                    var roles =  this._sysRoleRepository.List(roleSpec);
                    if (roles.Count > 0) throw new Exception(string.Format("角色名称[{0}],已经存在！", role.RoleName)); 
                    this._sysRoleRepository.Add(role);
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增角色!"),
                        CreateTime = DateTime.Now
                    });
                    
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
            }
        }


        public async Task AssignMenu(int roleId, List<int> menuIds)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Zero(roleId, nameof(roleId));
                    Guard.Against.NullOrEmpty(menuIds, nameof(menuIds));
                    RoleMenuSpecification rmSpec = new RoleMenuSpecification(roleId);
                    var rms =  this._roleMenuRepository.List(rmSpec);
                    if (rms.Count > 0)  this._roleMenuRepository.Delete(rms);
                    List<RoleMenu> addRoleMenus = new List<RoleMenu>();
                    menuIds.ForEach(async (mId) =>
                    {
                        RoleMenu roleMenu = new RoleMenu();
                        roleMenu.SysMenuId = mId;
                        roleMenu.SysRoleId = roleId;
                        addRoleMenus.Add(roleMenu);
                    });
                    this._roleMenuRepository.Add(addRoleMenus);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("角色[{0}]分配菜单!",roleId),
                        CreateTime = DateTime.Now
                    });
                    
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
               
            }
        }

        public async Task Enable(List<int> roleIds)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.NullOrEmpty(roleIds, nameof(roleIds));
                    var roles =  this._sysRoleRepository.ListAll();
                    List<SysRole> updRoles = new List<SysRole>();
                    roles.ForEach(role =>
                    {
                        if (roleIds.Contains(role.Id))
                        {
                            role.Status = Convert.ToInt32(SYSROLE_STATUS.正常);
                            updRoles.Add(role);
                        }
                    });
                    if (updRoles.Count > 0)
                        this._sysRoleRepository.Update(updRoles);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("启用角色!"),
                        CreateTime = DateTime.Now
                    });
                    
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
                
            }
        }

        public async Task Logout(List<int> roleIds)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.NullOrEmpty(roleIds, nameof(roleIds));
                    var roles =  this._sysRoleRepository.ListAll();
                    List<SysRole> updRoles = new List<SysRole>();
                    roles.ForEach(role =>
                    {
                        if (roleIds.Contains(role.Id))
                        {
                            role.Status = Convert.ToInt32(SYSROLE_STATUS.禁用);
                            updRoles.Add(role);
                        }
                    });
                    if (updRoles.Count > 0)
                         this._sysRoleRepository.Update(updRoles);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("禁用角色!"),
                        CreateTime = DateTime.Now
                    });
                    
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
               
            }
            
        }

        public async Task UpdateRole(int id, string roleName)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(roleName, nameof(roleName));
                    Guard.Against.Zero(id, nameof(id));
                    SysRoleSpecification roleSpec = new SysRoleSpecification(null, roleName);
                    var roles = this._sysRoleRepository.List(roleSpec);
                    if (roles.Count > 0) throw new Exception(string.Format("角色名称[{0}],已经存在！", roleName));
                    roleSpec = new SysRoleSpecification(id, null);
                    roles = this._sysRoleRepository.List(roleSpec);
                    if (roles.Count == 0) throw new Exception(string.Format("角色编号[{0}],不存在", id));
                    var role = roles[0];
                    role.RoleName = roleName;
                    this._sysRoleRepository.Update(role);

                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新角色!"),
                        CreateTime = DateTime.Now
                    });
                    
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }

            }
        }
    }
}
