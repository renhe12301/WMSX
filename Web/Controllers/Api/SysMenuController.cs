using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;
using Web.ViewModels.BasicInformation;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 系统菜单操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class SysMenuController:BaseApiController
    {
        private readonly ISysMenuViewModelService _sysMenuViewModelService;
        public SysMenuController(ISysMenuViewModelService sysMenuViewModelService)
        {
            this._sysMenuViewModelService = sysMenuViewModelService;
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <param name="roleId">当前角色编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMenus(int roleId)
        {
            var response = await this._sysMenuViewModelService.GetMenus(roleId);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 获取菜单树信息
        /// </summary>
        /// <param name="rootId">父类节点编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMenuTrees(int rootId)
        {
            var response = await this._sysMenuViewModelService.GetMenuTrees(rootId);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}
