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
        /// 获取库存组织
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        /// <param name="id">库存组织编号</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="whName">仓库名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetWarehouses(int? pageIndex, int? itemsPage,
                                      int? id, int? ouId, string whName)
        {
            var response = await this._warehouseViewModelService.GetWarehouses(pageIndex,
                itemsPage, id, ouId, whName);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 库存组织资产统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> WarehouseAssetChart(int ouId)
        {
            var response = await this._warehouseViewModelService.WarehouseAssetChart(ouId);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 库存组织物料统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> WarehouseMaterialChart(int ouId)
        {
            var response = await this._warehouseViewModelService.WarehouseMaterialChart(ouId);
            return Content(JsonConvert.SerializeObject(response));
        }
        /// <summary>
        /// 库存组织托盘统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> WarehouseTrayChart(int ouId)
        {
            var response = await this._warehouseViewModelService.WarehouseTrayChart(ouId);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 库存组织出入库记录统计
        /// </summary>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="inOutType">出入库类型 0-入库 1-出库</param>
        /// <param name="queryType">查询类型 0-当天 1-本周 2-本月 3-本季度 4-本年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> WarehouseEntryOutRecordChart(int ouId,int inOutType,int queryType)
        {
            var response = await this._warehouseViewModelService.WarehouseEntryOutRecordChart(ouId,inOutType,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }


    }
}
