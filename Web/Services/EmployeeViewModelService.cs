using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Services;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;

namespace Web.Services
{
    public class EmployeeViewModelService:IEmployeeViewModelService
    {

        private readonly IEmployeeService _employeeService;
        private readonly IAsyncRepository<Employee> _employeeRepository;
        private readonly IAsyncRepository<EmployeeOrg> _employeeOrgRepository;
        private readonly IAsyncRepository<EmployeeRole> _employeeRoleRepository;
        public EmployeeViewModelService(IEmployeeService employeeService,
                                        IAsyncRepository<EmployeeOrg> employeeOrgRepository,
                                        IAsyncRepository<Employee> employeeRepository,
                                        IAsyncRepository<EmployeeRole> employeeRoleRepository)
            
        {
            this._employeeService = employeeService;
            this._employeeOrgRepository = employeeOrgRepository;
            this._employeeRepository = employeeRepository;
            this._employeeRoleRepository = employeeRoleRepository;
        }

        public async Task<ResponseResultViewModel> AddEmployee(EmployeeViewModel employeeViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                Employee employee = new Employee
                {
                    LoginName = employeeViewModel.LoginName,
                    LoginPwd = employeeViewModel.LoginPwd,
                    Sex = employeeViewModel.Sex,
                    Address = employeeViewModel.Address,
                    Email = employeeViewModel.Email,
                    CreateTime = DateTime.Now,
                    Telephone = employeeViewModel.Telephone,
                    UserName = employeeViewModel.UserName
                };
                await this._employeeService.AddEmployee(employee);
                response.Data = employee.Id;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> AssignRole(EmployeeRoleViewModel employeeRoleViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._employeeService.AssignRole(employeeRoleViewModel.EmployeeId,employeeRoleViewModel.RoleIds);
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
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetEmployees(int? pageIndex,int? itemsPage,int? orgId,int? employeeId, string employeeName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                EmployeeRoleSpecification employeeRoleSpec = new EmployeeRoleSpecification(null, null, null);
                EmployeeOrgSpecification employeeOrgSpec = new EmployeeOrgSpecification(null, null, null);
                var employeeRoles = await this._employeeRoleRepository.ListAsync(employeeRoleSpec);
                var employeeOrgs = await this._employeeOrgRepository.ListAsync(employeeOrgSpec);
                
                if (orgId.HasValue)
                {
                    BaseSpecification<EmployeeOrg> baseSpecification = null;
                    if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                    {
                        baseSpecification = new EmployeeOrgPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                            orgId,employeeId, employeeName);
                    }
                    else
                    {
                        baseSpecification = new EmployeeOrgSpecification(orgId,employeeId, employeeName);
                    }
                    var employees = await this._employeeOrgRepository.ListAsync(baseSpecification);
                    List<EmployeeViewModel> employViewModels = new List<EmployeeViewModel>();
                    employees.ForEach(e =>
                    {
                        var roleNames = employeeRoles.FindAll(e => e.EmployeeId == e.Id)
                            .ConvertAll(e => e.SysRole.RoleName);
                        EmployeeViewModel employViewModel = new EmployeeViewModel
                        {
                            Id = e.Id,
                            UserName = e.Employee.UserName,
                            UserCode = e.Employee.UserCode,
                            Sex = e.Employee.Sex,
                            Telephone = e.Employee.Telephone,
                            Email = e.Employee.Email,
                            Address = e.Employee.Address,
                            Status = Enum.GetName(typeof(EMPLOYEE_STATUS), e.Employee.Status),
                            CreateTime = e.Employee.CreateTime.ToString(),
                            LoginName = e.Employee.LoginName,
                            LoginPwd = e.Employee.LoginPwd,
                            RoleName = string.Join('、', roleNames),
                            OrgName = e.Organization.OrgName,
                            Img = e.Employee.Img
                        };
                        employViewModels.Add(employViewModel);
                    });
                    if (pageIndex > -1 && itemsPage > 0)
                    {
                        var count = await this._employeeOrgRepository.CountAsync(
                            new EmployeeOrgSpecification(orgId,null, employeeName));
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
                else
                {
                    BaseSpecification<Employee> baseSpecification = null;
                    if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                    {
                        baseSpecification = new EmployeePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                            employeeId, employeeName);
                    }
                    else
                    {
                        baseSpecification = new EmployeeSpecification(employeeId, employeeName);
                    }

                    var employees = await this._employeeRepository.ListAsync(baseSpecification);
                    List<EmployeeViewModel> employViewModels = new List<EmployeeViewModel>();
                    employees.ForEach(e =>
                    {
                        var roleNames = employeeRoles.FindAll(e => e.EmployeeId == e.Id)
                            .ConvertAll(e => e.SysRole.RoleName);
                        var orgNames = employeeOrgs.FindAll(e => e.EmployeeId == e.Id)
                            .ConvertAll(e => e.Organization.OrgName);
                        EmployeeViewModel employViewModel = new EmployeeViewModel
                        {
                            Id = e.Id,
                            UserName = e.UserName,
                            UserCode = e.UserCode,
                            Sex = e.Sex,
                            Telephone = e.Telephone,
                            Email = e.Email,
                            Address = e.Address,
                            Status = Enum.GetName(typeof(EMPLOYEE_STATUS), e.Status),
                            CreateTime = e.CreateTime.ToString(),
                            LoginName = e.LoginName,
                            LoginPwd = e.LoginPwd,
                            RoleName = string.Join('、', roleNames),
                            OrgName = string.Join('、', orgNames),
                            Img = e.Img
                        };
                        employViewModels.Add(employViewModel);
                    });
                    if (pageIndex > -1 && itemsPage > 0)
                    {
                        var count = await this._employeeRepository.CountAsync(
                            new EmployeeSpecification(employeeId, employeeName));
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
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

      
        public async Task<ResponseResultViewModel> Logout(EmployeeViewModel employViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._employeeService.Logout(employViewModel.UserIds);
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
