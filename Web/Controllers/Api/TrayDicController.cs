using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels.BasicInformation;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 托盘字典API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class TrayDicController:BaseApiController
    {
        private readonly ITrayDicViewModelService _trayDicViewModelService;

        public TrayDicController(ITrayDicViewModelService trayDicViewModelService)
        {
            this._trayDicViewModelService = trayDicViewModelService;
        }

        /// <summary>
        /// 获取托盘字典信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页大小</param>
        /// <param name="id">托盘字典编号</param>
        /// <param name="trayCode">托盘字典编码</param>
        /// <param name="trayName">托盘名称</param>
        /// <param name="typeId">托盘类型编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTrayDics(int? pageIndex, int? itemsPage,
                                                      int? id, string trayCode,
                                                      string trayName,
                                                      int? typeId)
        {
            var response = await this._trayDicViewModelService.GetTrayDics(pageIndex,
                itemsPage, id, trayCode, trayName, typeId);
            return Ok(response);
        }


        /// <summary>
        /// 添加托盘字典
        /// </summary>
        /// <param name="trayDicViewModel">托盘字典对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTrayDic(TrayDicViewModel trayDicViewModel)
        {
            var response = await this._trayDicViewModelService.AddTrayDic(trayDicViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 更新托盘字典
        /// </summary>
        /// <param name="trayDicViewModel">托盘字典对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateTrayDic(TrayDicViewModel trayDicViewModel)
        {
            var response = await this._trayDicViewModelService.UpdateTrayDic(trayDicViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 删除托盘字典
        /// </summary>
        /// <param name="trayDicViewModel">托盘字典对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DelTrayDic(TrayDicViewModel trayDicViewModel)
        {
            var response = await this._trayDicViewModelService.DelTrayDic(trayDicViewModel);
            return Ok(response);
        }

    }
}
