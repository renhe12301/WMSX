using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;
using Web.ViewModels.OrganizationManager;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 系统角色操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class SysRoleController:BaseApiController
    {

        private readonly ISysRoleViewModelService _sysRoleViewModelService;

        public SysRoleController(ISysRoleViewModelService sysRoleViewModelService)
        {
            this._sysRoleViewModelService = sysRoleViewModelService;
        }


        /// <summary>
        /// 获取系统角色信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页大小</param>
        /// <param name="id">角色编号</param>
        /// <param name="parentId">父编号</param>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoles(int? pageIndex, int? itemsPage,
          int? id,int?parentId,string roleName)
        {
            var response = await this._sysRoleViewModelService.GetRoles(pageIndex,
                itemsPage, id,parentId,roleName);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 获取角色树信息
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="depthTag"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoleTrees(int rootId, string depthTag)
        {
            var response = await this._sysRoleViewModelService.GetRoleTrees(rootId, depthTag);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 添加系统角色
        /// </summary>
        /// <param name="sysRoleViewModel">系统角色对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddRole(SysRoleViewModel sysRoleViewModel)
        {
            var response = await this._sysRoleViewModelService.AddRole(sysRoleViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 更新系统角色
        /// </summary>
        /// <param name="sysRoleViewModel">系统角色对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRole(SysRoleViewModel sysRoleViewModel)
        {
            var response = await this._sysRoleViewModelService.UpdateRole(sysRoleViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }


        /// <summary>
        /// 注销系统角色
        /// </summary>
        /// <param name="sysRoleViewModel">系统角色对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout(SysRoleViewModel sysRoleViewModel)
        {
            var response = await this._sysRoleViewModelService.Logout(sysRoleViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 启用系统角色
        /// </summary>
        /// <param name="sysRoleViewModel">系统角色对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(SysRoleViewModel sysRoleViewModel)
        {
            var response = await this._sysRoleViewModelService.Enable(sysRoleViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 分配系统角色菜单
        /// </summary>
        /// <param name="roleMenuViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignMenu(RoleMenuViewModel roleMenuViewModel)
        {
            var response = await this._sysRoleViewModelService.AssignMenu(roleMenuViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}
