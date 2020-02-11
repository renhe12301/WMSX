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
    /// 物料字典操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class MaterialDicController:BaseApiController
    {
        private readonly IMaterialDicViewModelService _materialDicViewModelService;
        public MaterialDicController(IMaterialDicViewModelService materialDicViewModelService)
        {
            this._materialDicViewModelService = materialDicViewModelService;
        }

        /// <summary>
        /// 获取物料字典信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页大小</param>
        /// <param name="id">物料字典编号</param>
        /// <param name="materialCode">物料字典编码</param>
        /// <param name="materialName">物料字典名称</param>
        /// <param name="spec">物料字典规格</param>
        /// <param name="typeId">物料字典类型编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaterialDics(int? pageIndex, int? itemsPage,
                                                         int? id, string materialCode,
                                                         string materialName, string spec,
                                                         int? typeId)
        {
            var response = await this._materialDicViewModelService.GetMaterialDics(pageIndex, itemsPage,
                                         id, materialCode, materialName, spec, typeId);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}
