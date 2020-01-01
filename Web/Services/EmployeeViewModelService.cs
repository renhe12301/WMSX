using System;
using System.Collections.Generic;
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

        public EmployeeViewModelService(IEmployeeService employeeService,
                                        IAsyncRepository<EmployeeOrg> employeeOrgRepository,
                                        IAsyncRepository<Employee> employeeRepository)
            
        {
            this._employeeService = employeeService;
            this._employeeOrgRepository = employeeOrgRepository;
            this._employeeRepository = employeeRepository;
        }

        public async Task<ResponseResultViewModel> AddEmployee(EmployeeViewModel employeeViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                Employee employee = new Employee
                {
                    LoginName = employeeViewModel.LoginName,
                    LoinPwd = employeeViewModel.LoginPwd,
                    Sex = employeeViewModel.Sex,
                    Address = employeeViewModel.Address,
                    Email = employeeViewModel.Email,
                    CreateTime = DateTime.Now,
                    Telephone = employeeViewModel.Telephone,
                    UserName = employeeViewModel.UserName,
                    ParentId = employeeViewModel.ParentId,
                    Type = employeeViewModel.Type
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
                await this._employeeService.Enable(employViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetEmployees(int? pageIndex,int? itemsPage,int? employeeId, string userName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Employee> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new EmployeePaginatedSpecification(pageIndex.Value,itemsPage.Value,
                        employeeId,userName);
                }
                else
                {
                    baseSpecification = new EmployeeSpecification(employeeId, userName);
                }
                var employees = await this._employeeRepository.ListAsync(baseSpecification);
                List<EmployeeViewModel> employViewModels = new List<EmployeeViewModel>();
                employees.ForEach(e =>
                {
                    EmployeeViewModel employViewModel = new EmployeeViewModel
                    {
                        Id = e.Id,
                        UserName = e.UserName,
                        Sex = e.Sex,
                        Telephone = e.Telephone,
                        Email=e.Email,
                        Address=e.Address,
                        Status= Enum.GetName(typeof(EMPLOYEE_STATUS), e.Status),
                        CreateTime=e.CreateTime.ToString(),
                        LoginName=e.LoginName,
                        LoginPwd=e.LoinPwd
                    };
                });
                response.Data = employViewModels;
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
                await this._employeeService.Logout(employViewModel.Id);
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
