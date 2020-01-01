using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels.BasicInformation;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 货位信息操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class LocationController:BaseApiController
    {

        private readonly ILocationViewModelService _locationViewModelService;

        public LocationController(ILocationViewModelService locationViewModelService)
        {
            this._locationViewModelService = locationViewModelService;
        }

        /// <summary>
        /// 添加货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationViewModel locationViewModel)
        {
            var response = await this._locationViewModelService.AddLocation(locationViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 生成货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BuildLocation(LocationViewModel locationViewModel)
        {
            var response = await this._locationViewModelService.BuildLocation(locationViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 启用货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(LocationViewModel locationViewModel)
        {
            var response = await this._locationViewModelService.Enable(locationViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 禁用货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Disable(LocationViewModel locationViewModel)
        {
            var response = await this._locationViewModelService.Disable(locationViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 清理货位上物料托盘信息
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Clear(LocationViewModel locationViewModel)
        {
            var response = await this._locationViewModelService.Clear(locationViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 锁定货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Lock(LocationViewModel locationViewModel)
        {
            var response = await this._locationViewModelService.Lock(locationViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 解锁货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnLock(LocationViewModel locationViewModel)
        {
            var response = await this._locationViewModelService.UnLock(locationViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 更新货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateLocation(LocationViewModel locationViewModel)
        {
            var response = await this._locationViewModelService.UpdateLocation(locationViewModel);
            return Ok(response);
        }

    }
}
