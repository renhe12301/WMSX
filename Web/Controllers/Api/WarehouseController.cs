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
    /// 仓库操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class WarehouseController:BaseApiController
    {
        private readonly IWarehouseViewModelService _warehouseViewModelService;

        public WarehouseController(IWarehouseViewModelService warehouseViewModelService)
        {
            this._warehouseViewModelService = warehouseViewModelService;
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        /// <param name="id">仓库编号</param>
        /// <param name="orgId">组织编号</param>
        /// <param name="whName">仓库名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetWarehouses(int? pageIndex, int? itemsPage,
                                      int? id, int? orgId, string whName)
        {
            var response = await this._warehouseViewModelService.GetWarehouses(pageIndex,
                itemsPage, id, orgId, whName);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 添加仓库
        /// </summary>
        /// <param name="warehouseViewModel">仓库实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddWarehouse(WarehouseViewModel warehouseViewModel)
        {
            var response = await this._warehouseViewModelService.AddWarehouse(warehouseViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 更新仓库
        /// </summary>
        /// <param name="warehouseViewModel">仓库实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateWarehouse(WarehouseViewModel warehouseViewModel)
        {
            var response = await this._warehouseViewModelService.UpdateWarehouse(warehouseViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 禁用仓库
        /// </summary>
        /// <param name="warehouseViewModel">仓库实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Disable(WarehouseViewModel warehouseViewModel)
        {
            var response = await this._warehouseViewModelService.Disable(warehouseViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 启用仓库
        /// </summary>
        /// <param name="warehouseViewModel">仓库实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(WarehouseViewModel warehouseViewModel)
        {
            var response = await this._warehouseViewModelService.Enable(warehouseViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

    }
}
