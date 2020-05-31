using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.AuthorityManager;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using ApplicationCore.Misc;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IAsyncRepository<Employee> _employeeRepository;
        private readonly IAsyncRepository<EmployeeRole> _employeeRoleRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        public EmployeeService(IAsyncRepository<Employee> employeeRepository,
                               IAsyncRepository<EmployeeRole> employeeRoleRepository,
                               IAsyncRepository<LogRecord> logRecordRepository
                               )
        {
            this._employeeRepository = employeeRepository;
            this._employeeRoleRepository = employeeRoleRepository;
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddEmployee(Employee employee,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(employee, nameof(employee));
                    Guard.Against.Zero(employee.Id, nameof(employee.Id));
                    Guard.Against.NullOrEmpty(employee.UserCode, nameof(employee.UserCode));
                    Guard.Against.NullOrEmpty(employee.UserName, nameof(employee.UserName));
                    if (unique)
                    {
                        EmployeeSpecification employeeSpec = new EmployeeSpecification(employee.Id, null, null, null);
                        var employees = await this._employeeRepository.ListAsync(employeeSpec);
                        if (employees.Count == 0)
                            this._employeeRepository.Add(employee);
                    }
                    else
                    {
                        this._employeeRepository.Add(employee);
                    }
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增员工!"),
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

        public async Task UpdateEmployee(Employee employee)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(employee, nameof(employee));
                    this._employeeRepository.Update(employee);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新员工[{0}]!",employee.Id),
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

        public async Task AssignRole(int employeeId,List<int> roleIds)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Zero(employeeId, nameof(employeeId));

                    EmployeeRoleSpecification erSpec = new EmployeeRoleSpecification(null, employeeId, null);
                    var emRoles =  this._employeeRoleRepository.List(erSpec);
                    if (emRoles.Count > 0)
                        this._employeeRoleRepository.Delete(emRoles);
                    List<EmployeeRole> ers = new List<EmployeeRole>();
                    roleIds.ForEach(async (roleId) =>
                    {
                        EmployeeRole employeeRole = new EmployeeRole();
                        employeeRole.EmployeeId = employeeId;
                        employeeRole.SysRoleId = roleId;
                        ers.Add(employeeRole);
                    });
                    this._employeeRoleRepository.Add(ers);
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("分配员工[{0}]角色!",employeeId),
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

        public async Task Enable(List<int> userIds)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.NullOrEmpty(userIds, nameof(userIds));
                    var users =  this._employeeRepository.ListAll();
                    List<Employee> updEmployees = new List<Employee>();
                    users.ForEach(em =>
                    {
                        if (userIds.Contains(em.Id))
                        {
                            em.Status = Convert.ToInt32(EMPLOYEE_STATUS.正常);
                            updEmployees.Add(em);
                        }
                    });
                    if (updEmployees.Count > 0)
                        this._employeeRepository.Update(updEmployees);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("启用员工!"),
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

        public async Task Logout(List<int> userIds)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.NullOrEmpty(userIds, nameof(userIds));
                    var users =  this._employeeRepository.ListAll();
                    List<Employee> updEmployees = new List<Employee>();
                    users.ForEach(em =>
                    {
                        if (userIds.Contains(em.Id))
                        {
                            em.Status = Convert.ToInt32(EMPLOYEE_STATUS.禁用);
                            updEmployees.Add(em);
                        }
                    });
                    if (updEmployees.Count > 0)
                         this._employeeRepository.Update(updEmployees);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("禁用员工!"),
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
        
        
        public async Task AddEmployee(List<Employee> employees,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(employees, nameof(employees));
                    Guard.Against.NullOrEmpty(employees, nameof(employees));
                    if (unique)
                    {
                        List<Employee> adds = new List<Employee>();
                        employees.ForEach(async (em) =>
                        {
                            EmployeeSpecification employeeSpec = new EmployeeSpecification(em.Id, null, null, null);
                            var findEmployees =  this._employeeRepository.List(employeeSpec);
                            if (findEmployees.Count > 0)
                                adds.Add(employees.First());
                        });
                        if (adds.Count > 0)
                             this._employeeRepository.Add(adds);
                    }
                    else
                    {
                         this._employeeRepository.Add(employees);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增员工!"),
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

        public async Task UpdateEmployee(List<Employee> employees)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(employees, nameof(employees));
                    this._employeeRepository.Update(employees);
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新员工!"),
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
