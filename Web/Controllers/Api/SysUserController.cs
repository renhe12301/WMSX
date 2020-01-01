using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels.OrganizationManager;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 系统用户操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class SysUserController:BaseApiController
    {
        private readonly ISysUserViewModelService _sysUserViewModelService;

        public SysUserController(ISysUserViewModelService sysUserViewModelService)
        {
            this._sysUserViewModelService = sysUserViewModelService;
        }

        /// <summary>
        /// 获取系统用户信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        /// <param name="id">系统用户编号</param>
        /// <param name="employeeId">员工编号</param>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers(int? pageIndex, int? itemsPage,
                                      int? id, int? employeeId, string loginName)
        {
            var response = await this._sysUserViewModelService.GetUsers(pageIndex,
                itemsPage, id,employeeId,loginName);
            return Ok(response);
        }

        /// <summary>
        /// 添加系统用户
        /// </summary>
        /// <param name="sysUserViewModel">系统用户对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(SysUserViewModel sysUserViewModel)
        {
            var response = await this._sysUserViewModelService.AddUser(sysUserViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 更新系统用户
        /// </summary>
        /// <param name="sysUserViewModel">系统用户对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUser(SysUserViewModel sysUserViewModel)
        {
            var response = await this._sysUserViewModelService.UpdateUser(sysUserViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 注销帐户
        /// </summary>
        /// <param name="sysUserViewModel">系统用户对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout(SysUserViewModel sysUserViewModel)
        {
            var response = await this._sysUserViewModelService.Logout(sysUserViewModel);
            return Ok(response);
        }


        /// <summary>
        /// 启用用户
        /// </summary>
        /// <param name="sysUserViewModel">系统用户对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(SysUserViewModel sysUserViewModel)
        {
            var response = await this._sysUserViewModelService.Enable(sysUserViewModel);
            return Ok(response);
        }
    }
}
