using System;
using Web.Interfaces;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Interfaces;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using ApplicationCore.Misc;

namespace Web.Services
{
    public class SysUserViewModelService:ISysUserViewModelService
    {
        private readonly IAsyncRepository<SysUser> _sysUserRepository;
        private readonly ISysUserService  _sysUserService;

        public SysUserViewModelService(IAsyncRepository<SysUser> sysUserRepository,
                                       ISysUserService sysUserService)
        {
            this._sysUserRepository = sysUserRepository;
            this._sysUserService = sysUserService;
        }

        public async Task<ResponseResultViewModel> GetUsers(int? pageIndex, int? itemsPage, int? id, int? employeeId, string loginName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<SysUser> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SysUserPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, employeeId, loginName);
                }
                else
                {
                    baseSpecification = new SysUserSpecification(id, employeeId, loginName);
                }
                var users = await this._sysUserRepository.ListAsync(baseSpecification);
                List<SysUserViewModel> userViewModels = new List<SysUserViewModel>();
                users.ForEach(e =>
                {
                    SysUserViewModel userViewModel = new SysUserViewModel
                    {
                        Id = e.Id,
                        LoginName = e.LoginName,
                        LoginPwd = e.LoginPwd,
                        CreateTime = e.CreateTime.ToString(),
                        EmployeeName = e.Employee.UserName,
                        Status = Enum.GetName(typeof(SYSUSER_STATUS), e.Status)
                    };
                    userViewModels.Add(userViewModel);
                });
                response.Data = userViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> AddUser(SysUserViewModel userViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                SysUser sysUser = new SysUser
                {
                    CreateTime = DateTime.Now,
                    EmployeeId = userViewModel.EmployeeId,
                    LoginName = userViewModel.LoginName,
                    LoginPwd = userViewModel.LoginPwd
                };
                await this._sysUserService.AddUser(sysUser);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Enable(SysUserViewModel userViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._sysUserService.Enable(userViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

       

        public async Task<ResponseResultViewModel> Logout(SysUserViewModel userViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._sysUserService.Logout(userViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> UpdateUser(SysUserViewModel userViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._sysUserService.UpdateUser(userViewModel.Id,userViewModel.LoginName);
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
