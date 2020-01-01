using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 物料类型API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class MaterialTypeController:BaseApiController
    {
        private readonly IMaterialTypeViewModelService _materialTypeViewModelService;

        public MaterialTypeController(IMaterialTypeViewModelService materialTypeViewModelService)
        {
            this._materialTypeViewModelService = materialTypeViewModelService;
        }

        /// <summary>
        /// 添加物料类型
        /// </summary>
        /// <param name="materialTypeViewModel">物料类型实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddMaterialType(MaterialTypeViewModel materialTypeViewModel)
        {
            var response = await this._materialTypeViewModelService.AddMaterialType(materialTypeViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 更新物料类型
        /// </summary>
        /// <param name="materialTypeViewModel">物料类型实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateMaterialType(MaterialTypeViewModel materialTypeViewModel)
        {
            var response = await this._materialTypeViewModelService.UpdateMaterialType(materialTypeViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 删除物料类型
        /// </summary>
        /// <param name="materialTypeViewModel">物料类型实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DelMaterialType(MaterialTypeViewModel materialTypeViewModel)
        {
            var response = await this._materialTypeViewModelService.DelMaterialType(materialTypeViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 获取物料类型
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">页面显示大小</param>
        /// <param name="id">物料类型编号</param>
        /// <param name="parentId">物料父级编号</param>
        /// <param name="typeName">物料类型名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaterialTypes(int? pageIndex, int? itemsPage, int? id, int? parentId, string typeName)
        {
            var response = await this._materialTypeViewModelService.GetMaterialTypes(pageIndex, itemsPage, id, parentId, typeName);
            return Ok(response);
        }

        /// <summary>
        /// 获取物料类型树
        /// </summary>
        /// <param name="rootId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaterialTypeTree(int rootId)
        {
            var response = await this._materialTypeViewModelService.GetMaterialTypeTree(rootId);
            return Ok(response);
        }
    }
}
