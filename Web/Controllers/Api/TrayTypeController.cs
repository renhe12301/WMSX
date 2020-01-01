using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 托盘类型API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class TrayTypeController:BaseApiController
    {
        private ITrayTypeViewModelService _trayTypeViewModelService;

        public TrayTypeController(ITrayTypeViewModelService trayTypeViewModelService)
        {
            this._trayTypeViewModelService = trayTypeViewModelService;
        }

        /// <summary>
        /// 添加托盘字典类型
        /// </summary>
        /// <param name="trayTypeViewModel">托盘字典类型实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTrayType(TrayTypeViewModel trayTypeViewModel)
        {
            var response = await this._trayTypeViewModelService.AddTrayType(trayTypeViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 分配托盘字典到类型
        /// </summary>
        /// <param name="trayTypeViewModel">托盘字典类型实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignTrayDic(TrayTypeViewModel trayTypeViewModel)
        {
            var response = await this._trayTypeViewModelService.AssignTrayDic(trayTypeViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 删除托盘字典类型
        /// </summary>
        /// <param name="trayTypeViewModel">托盘字典类型实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DelTrayType(TrayTypeViewModel trayTypeViewModel)
        {
            var response = await this._trayTypeViewModelService.DelTrayType(trayTypeViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 更新托盘字典类型
        /// </summary>
        /// <param name="trayTypeViewModel">托盘字典类型实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateTrayType(TrayTypeViewModel trayTypeViewModel)
        {
            var response = await this._trayTypeViewModelService.UpdateTrayType(trayTypeViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 获取托盘字典类型
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页大小</param>
        /// <param name="id">托盘字典编号</param>
        /// <param name="parentId">类型父类型编号</param>
        /// <param name="typeName">类型名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetTrayTypes(int? pageIndex, int? itemsPage, int? id, int? parentId, string typeName)
        {
            var response = await this._trayTypeViewModelService.GetTrayTypes(pageIndex,itemsPage,id,parentId,typeName);
            return Ok(response);
        }

        /// <summary>
        /// 获取托盘字典类型树
        /// </summary>
        /// <param name="rootId">根节点编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTrayTypeTree(int rootId)
        {
            var response = await this._trayTypeViewModelService.GetTrayTypeTree(rootId);
            return Ok(response);
        }

    }
}
