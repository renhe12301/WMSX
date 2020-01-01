using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using System.Collections.Generic;
using Web.ViewModels.BasicInformation;
using Microsoft.AspNetCore.Cors;

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
        /// 添加库区
        /// </summary>
        /// <param name="reservoirAreaViewModel">库区实体对象</param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> AddArea(ReservoirAreaViewModel reservoirAreaViewModel)
        {
            var response = await this._reservoirAreaViewModelService.AddArea(reservoirAreaViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 更新库区信息
        /// </summary>
        /// <param name="reservoirAreaViewModel">库区实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateArea(ReservoirAreaViewModel reservoirAreaViewModel)
        {
            var response = await this._reservoirAreaViewModelService.UpdateArea(reservoirAreaViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 启用库区
        /// </summary>
        /// <param name="reservoirAreaViewModel">库区实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(ReservoirAreaViewModel reservoirAreaViewModel)
        {
            var response = await this._reservoirAreaViewModelService.Enable(reservoirAreaViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 禁用库区
        /// </summary>
        /// <param name="reservoirAreaViewModel">库区实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Disable(ReservoirAreaViewModel reservoirAreaViewModel)
        {
            var response = await this._reservoirAreaViewModelService.Disable(reservoirAreaViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 分配货位
        /// </summary>
        /// <param name="locationViewModel">库区货位关系对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignLocation(LocationViewModel locationViewModel)
        {
            var response = await this._reservoirAreaViewModelService.AssignLocation(locationViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 分配物料字典
        /// </summary>
        /// <param name="materialDicTypeAreaViewModel">库区物料关系对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignMaterialType(MaterialDicTypeAreaViewModel materialDicTypeAreaViewModel)
        {
            var response = await this._reservoirAreaViewModelService.AssignMaterialType(materialDicTypeAreaViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 获取库区信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        /// <param name="id">库区编号</param>
        /// <param name="pid">库区父级编号</param>
        /// <param name="wareHouseId">仓库编号</param>
        /// <param name="areaName">库区名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAreas(int? pageIndex, int? itemsPage, int? id, int? pid, int? wareHouseId,int? type,  string areaName)
        {
            var response = await this._reservoirAreaViewModelService.GetAreas(pageIndex, itemsPage,id,pid,wareHouseId,type, areaName);
            return Ok(response);
        }


    }
}
