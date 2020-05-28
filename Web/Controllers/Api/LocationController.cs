using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        /// 获取货位信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页大小</param>
        /// <param name="id">货位编号</param>
        /// <param name="sysCode">货位系统编码</param>
        /// <param name="userCode">货位用户编码</param>
        /// <param name="types">货位类型</param>
        /// <param name="phyId">物理仓库编号</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="wareHouseId">仓库编号</param>
        /// <param name="areaId">库区编号</param>
        /// <param name="status">货位状态</param>
        /// <param name="inStocks">是否有货</param>
        /// <param name="isTasks">是否有任务</param>
        /// <param name="floors">层</param>
        /// <param name="items">排</param>
        /// <param name="cols">列</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLocations(int? pageIndex, int? itemsPage, int? id,
            string sysCode,string userCode,string types, int? phyId,int? ouId, int? wareHouseId, int? areaId,  string status,
            string inStocks,string isTasks,string floors,string items,string cols)
        {
            var response = await this._locationViewModelService.GetLocations(pageIndex, itemsPage, id, sysCode,userCode,types,
                phyId,ouId, wareHouseId, areaId,  status,inStocks,isTasks,floors,items,cols);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 获取最大的层、排、列
        /// </summary>
        /// <param name="phyId">所属实体仓库编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaxFloorItemCol(int phyId)
        {
            var response = await this._locationViewModelService.GetMaxFloorItemCol(phyId);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 获取最大的层
        /// </summary>
        /// <param name="phyId">所属实体仓库编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaxFloor(int phyId)
        {
            var response = await this._locationViewModelService.GetMaxFloor(phyId);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 获取最大的排
        /// </summary>
        /// <param name="phyId">所属实体仓库编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaxItem(int phyId)
        {
            var response = await this._locationViewModelService.GetMaxItem(phyId);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 获取最大的列
        /// </summary>
        /// <param name="phyId">所属实体仓库编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaxCol(int phyId)
        {
            var response = await this._locationViewModelService.GetMaxCol(phyId);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 添加货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationViewModel locationViewModel)
        {
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                locationViewModel.Tag = cookie.loginName;
            }
            var response = await this._locationViewModelService.AddLocation(locationViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 生成货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BuildLocation(LocationViewModel locationViewModel)
        { 
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                locationViewModel.Tag = cookie.loginName;
            }
            var response = await this._locationViewModelService.BuildLocation(locationViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 启用货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(LocationViewModel locationViewModel)
        {
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                locationViewModel.Tag = cookie.loginName;
            }
            var response = await this._locationViewModelService.Enable(locationViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 禁用货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Disable(LocationViewModel locationViewModel)
        {
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                locationViewModel.Tag = cookie.loginName;
            }
            var response = await this._locationViewModelService.Disable(locationViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 清理货位上物料托盘信息
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Clear(LocationViewModel locationViewModel)
        {
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                locationViewModel.Tag = cookie.loginName;
            }
            var response = await this._locationViewModelService.Clear(locationViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 更新货位
        /// </summary>
        /// <param name="locationViewModel">货位对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateLocation(LocationViewModel locationViewModel)
        {
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                locationViewModel.Tag = cookie.loginName;
            }
            var response = await this._locationViewModelService.UpdateLocation(locationViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

    }
}
