using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels.BasicInformation;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 物料字典单位API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class MaterialUnitController:BaseApiController
    {
        private readonly IMaterialUnitViewModelService _materialUnitViewModelService;

        public MaterialUnitController(IMaterialUnitViewModelService materialUnitViewModelService)
        {
            this._materialUnitViewModelService = materialUnitViewModelService;
        }

        /// <summary>
        /// 添加物料字典单位
        /// </summary>
        /// <param name="materialUnitViewModel">物料单位实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddMaterialUnit(MaterialUnitViewModel materialUnitViewModel)
        {
            var response = await this._materialUnitViewModelService.AddMaterialUnit(materialUnitViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 更新物料字典单位
        /// </summary>
        /// <param name="materialUnitViewModel">物料单位实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateMaterialUnit(MaterialUnitViewModel materialUnitViewModel)
        {
            var response = await this._materialUnitViewModelService.UpdateMaterialUnit(materialUnitViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 删除物料字典单位
        /// </summary>
        /// <param name="materialUnitViewModel">物料单位实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DelMaterialUnit(MaterialUnitViewModel materialUnitViewModel)
        {
            var response = await this._materialUnitViewModelService.DelMaterialUnit(materialUnitViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 获取物料单位信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页大小</param>
        /// <param name="id">物料单位编号</param>
        /// <param name="unitName">物料单位名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaterialUnits(int? pageIndex, int? itemsPage, int? id, string unitName)
        {
            var response = await this._materialUnitViewModelService.GetMaterialUnits(pageIndex,id , itemsPage, unitName);
            return Ok(response);
        }

    }
}
