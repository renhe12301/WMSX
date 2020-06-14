using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.AuthorityManager;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Services;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using Web.ViewModels.AuthorityManager;

namespace Web.Services
{
    public class EmployeeViewModelService:IEmployeeViewModelService
    {

        private readonly IEmployeeService _employeeService;
        private readonly IAsyncRepository<Employee> _employeeRepository;
        private readonly IAsyncRepository<EmployeeRole> _employeeRoleRepository;
        private readonly ILogRecordService _logRecordService;
        public EmployeeViewModelService(IEmployeeService employeeService,
                                        IAsyncRepository<Employee> employeeRepository,
                                        IAsyncRepository<EmployeeRole> employeeRoleRepository,
                                        ILogRecordService logRecordService)
            
        {
            this._employeeService = employeeService;
            this._employeeRepository = employeeRepository;
            this._employeeRoleRepository = employeeRoleRepository;
            this._logRecordService = logRecordService;
        }

       
        public async Task<ResponseResultViewModel> AssignRole(EmployeeViewModel employeeViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._employeeService.AssignRole(employeeViewModel.Id,employeeViewModel.RoleIds);
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                    LogDesc = string.Format("给用户[{0}],分配角色[{1}]",employeeViewModel.Id,
                              string.Join(',',employeeViewModel.RoleIds.ConvertAll(e=>e))),
                    Founder = employeeViewModel.Tag?.ToString(),
                    CreateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetRoles(int employeeId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                EmployeeRoleSpecification employeeRoleSpec=new EmployeeRoleSpecification(null,employeeId,null);
                var employeeRoles = await this._employeeRoleRepository.ListAsync(employeeRoleSpec);
                List<SysRoleViewModel> roleViewModels=new List<SysRoleViewModel>();
                employeeRoles.ForEach(e =>
                {
                    SysRoleViewModel roleViewModel = new SysRoleViewModel
                    {
                        Id = e.SysRoleId,
                        RoleName = e.SysRole.RoleName
                    };
                    roleViewModels.Add(roleViewModel);
                });
                response.Data = roleViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
               
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Enable(EmployeeViewModel employViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._employeeService.Enable(employViewModel.UserIds);
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                    LogDesc = string.Format("启用用户[{0}]",string.Join(',',employViewModel.UserIds.ConvertAll(e=>e))),
                    Founder = employViewModel.Tag?.ToString(),
                    CreateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetEmployees(int? pageIndex,int? itemsPage,int? depId,int? employeeId,
                                                   string employeeName,string userCode)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                EmployeeSpecification employeeSpec =
                    new EmployeeSpecification(employeeId, depId, employeeName, userCode);
                var employees = await this._employeeRepository.ListAsync(employeeSpec);

                EmployeeRoleSpecification employeeRoleSpec = new EmployeeRoleSpecification(null, null, null);
                var employeeRoles = await this._employeeRoleRepository.ListAsync(employeeRoleSpec);

                BaseSpecification<Employee> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new EmployeePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        employeeId, depId, employeeName, userCode);
                }
                else
                {
                    baseSpecification = new EmployeeSpecification(employeeId, depId, employeeName, userCode);
                }

                List<EmployeeViewModel> employViewModels = new List<EmployeeViewModel>();
                employees.ForEach(e =>
                {
                    string md5 = e.MD5String();
                    var roleNames = employeeRoles.FindAll(er => er.EmployeeId == e.Id)
                        .ConvertAll(er => er.SysRole.RoleName);

                    EmployeeViewModel employViewModel = new EmployeeViewModel();
                    employViewModel.Id = e.Id;
                    employViewModel.UserName = e.UserName;
                    employViewModel.UserCode = e.UserCode;
                    employViewModel.Sex = e.Sex;
                    employViewModel.Telephone = e.Telephone;
                    employViewModel.Email = e.Email;
                    if(e.Status.HasValue)
                    employViewModel.Status = Enum.GetName(typeof(EMPLOYEE_STATUS), e.Status);
                    employViewModel.CreateTime = e.CreateTime.ToString();
                    employViewModel.LoginName = e.LoginName;
                    employViewModel.LoginPwd = e.LoginPwd;
                    employViewModel.RoleName = string.Join('、', roleNames);
                    employViewModel.OrgName = e.Organization?.OrgName;
                    employViewModel.OrgId = e.OrganizationId.GetValueOrDefault();
                    employViewModel.Img = e.Img;
                    employViewModels.Add(employViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._employeeRepository.CountAsync(
                        new EmployeeSpecification(employeeId, depId, employeeName, userCode));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = employViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = employViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }


        public async Task<ResponseResultViewModel> Login(EmployeeViewModel employViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                EmployeeLoginSpecification employeeLoginSpec = new EmployeeLoginSpecification(employViewModel.LoginName,employViewModel.LoginPwd);
                List<Employee> employees = await this._employeeRepository.ListAsync(employeeLoginSpec);
                if(employees.Count==0)throw new Exception("用户名或者密码错误,登录失败！");
                EmployeeRoleSpecification employeeRoleSpec = new EmployeeRoleSpecification(null,employees[0].Id,null);
                List<EmployeeRole> employeeRoles = await this._employeeRoleRepository.ListAsync(employeeRoleSpec);
                dynamic dym = new ExpandoObject();
                dym.Roles = employeeRoles.ConvertAll(e => e.SysRole);
                dym.OrgId = employees[0].OrganizationId.GetValueOrDefault();
                dym.UserId = employees[0].Id;
                dym.UserName = employees[0].UserName;
                dym.UserCode = employees[0].UserCode;
                response.Data = dym;
                
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.登录日志),
                    LogDesc = string.Format("登录系统！"),
                    Founder = employViewModel.LoginName,
                    CreateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Logout(EmployeeViewModel employViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._employeeService.Logout(employViewModel.UserIds);
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                    LogDesc = string.Format("注销用户[{0}]",string.Join(',',employViewModel.UserIds.ConvertAll(e=>e))),
                    Founder = employViewModel.Tag?.ToString(),
                    CreateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
            return response;
        }
    }
}
