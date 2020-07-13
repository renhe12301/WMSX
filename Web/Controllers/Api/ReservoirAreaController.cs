using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using System.Collections.Generic;
using Web.ViewModels.BasicInformation;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 库区操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class ReservoirAreaController : BaseApiController
    {
        private readonly IReservoirAreaViewModelService _reservoirAreaViewModelService;

        public ReservoirAreaController(IReservoirAreaViewModelService reservoirAreaViewModelService)
        {
            this._reservoirAreaViewModelService = reservoirAreaViewModelService;
        }
        

        /// <summary>
        /// 分配货位
        /// </summary>
        /// <param name="locationViewModel">库区货位关系对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignLocation(LocationViewModel locationViewModel)
        {
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                locationViewModel.Tag = cookie.loginName;
            }
            var response = await this._reservoirAreaViewModelService.AssignLocation(locationViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 获取库区信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        /// <param name="id">库区编号</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="wareHouseId">仓库编号</param>
        /// <param name="ownerType">所有者类型</param>
        /// <param name="areaName">库区名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAreas(int? pageIndex, int? itemsPage, int? id,int? ouId,int? wareHouseId,string ownerType,  string areaName)
        {
            var response = await this._reservoirAreaViewModelService.GetAreas(pageIndex, itemsPage,id,ouId,wareHouseId, ownerType, areaName);
            return Content(JsonConvert.SerializeObject(response));
        }
 

        /// <summary>
        /// 设置库区类型
        /// </summary>
        /// <param name="reservoirAreaViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetOwnerType(ReservoirAreaViewModel reservoirAreaViewModel)
        {
          
            var response = await this._reservoirAreaViewModelService.SetOwnerType(reservoirAreaViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

    }
}
