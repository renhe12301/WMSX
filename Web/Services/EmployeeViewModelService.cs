﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.AuthorityManager;
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
        public EmployeeViewModelService(IEmployeeService employeeService,
                                        IAsyncRepository<Employee> employeeRepository,
                                        IAsyncRepository<EmployeeRole> employeeRoleRepository)
            
        {
            this._employeeService = employeeService;
            this._employeeRepository = employeeRepository;
            this._employeeRoleRepository = employeeRoleRepository;
        }

       
        public async Task<ResponseResultViewModel> AssignRole(EmployeeViewModel employeeViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._employeeService.AssignRole(employeeViewModel.Id,employeeViewModel.RoleIds);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
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
                EmployeeSpecification employeeSpec = new EmployeeSpecification(employeeId, orgId, employeeName);
                var employeeRoles = await this._employeeRoleRepository.ListAsync(employeeRoleSpec);
                var employees = await this._employeeRepository.ListAsync(employeeSpec);
                
                if (orgId.HasValue)
                {
                    BaseSpecification<Employee> baseSpecification = null;
                    if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                    {
                        baseSpecification = new EmployeePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                            employeeId, orgId, employeeName);
                    }
                    else
                    {
                        baseSpecification = new EmployeeSpecification(employeeId,orgId, employeeName);
                    }
                    List<EmployeeViewModel> employViewModels = new List<EmployeeViewModel>();
                    employees.ForEach(e =>
                    {
                        var roleNames = employeeRoles.FindAll(er => er.EmployeeId == e.Id)
                            .ConvertAll(er => er.SysRole.RoleName);
                        EmployeeViewModel employViewModel = new EmployeeViewModel
                        {
                            Id = e.Id,
                            UserName = e.UserName,
                            UserCode = e.UserCode,
                            Sex = e.Sex,
                            Telephone = e.Telephone,
                            Email = e.Email,
                            Status = Enum.GetName(typeof(EMPLOYEE_STATUS), e.Status),
                            CreateTime = e.CreateTime.ToString(),
                            LoginName = e.LoginName,
                            LoginPwd = e.LoginPwd,
                            RoleName = string.Join('、', roleNames),
                            OrgName = e.Organization.OrgName,
                            Img = e.Img
                        };
                        employViewModels.Add(employViewModel);
                    });
                    if (pageIndex > -1 && itemsPage > 0)
                    {
                        var count = await this._employeeRepository.CountAsync(
                            new EmployeeSpecification(orgId,orgId, employeeName));
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
                            employeeId,orgId, employeeName);
                    }
                    else
                    {
                        baseSpecification = new EmployeeSpecification(employeeId, orgId,employeeName);
                    }
                    
                    List<EmployeeViewModel> employViewModels = new List<EmployeeViewModel>();
                    employees.ForEach(e =>
                    {
                        var roleNames = employeeRoles.FindAll(er => er.EmployeeId == e.Id)
                            .ConvertAll(er => er.SysRole.RoleName);
                       
                        EmployeeViewModel employViewModel = new EmployeeViewModel
                        {
                            Id = e.Id,
                            UserName = e.UserName,
                            UserCode = e.UserCode,
                            Sex = e.Sex,
                            Telephone = e.Telephone,
                            Email = e.Email,
                            Status = Enum.GetName(typeof(EMPLOYEE_STATUS), e.Status),
                            CreateTime = e.CreateTime.ToString(),
                            LoginName = e.LoginName,
                            LoginPwd = e.LoginPwd,
                            RoleName = string.Join('、', roleNames),
                            OrgName = e.Organization.OrgName,
                            Img = e.Img
                        };
                        employViewModels.Add(employViewModel);
                    });
                    if (pageIndex > -1 && itemsPage > 0)
                    {
                        var count = await this._employeeRepository.CountAsync(
                            new EmployeeSpecification(employeeId, orgId,employeeName));
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
