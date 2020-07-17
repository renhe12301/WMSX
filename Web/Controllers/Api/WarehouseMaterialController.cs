using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 仓库物料操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class WarehouseMaterialController:BaseApiController
    {
        private readonly IWarehouseMaterialViewModelService _warehouseMaterialViewModelService;
        public WarehouseMaterialController(IWarehouseMaterialViewModelService warehouseMaterialViewModelService)
        {
            this._warehouseMaterialViewModelService = warehouseMaterialViewModelService;
        }


        /// <summary>
        /// 获取仓库物料信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页大小</param>
        /// <param name="id">仓库物料编号</param>
        /// <param name="materialCode">关联物料字典编码</param>
        /// <param name="materialDicId">关联物料字典编号</param>
        /// <param name="materialName">关联物料字典名称</param>
        /// <param name="materialSpec">关联物料字典属性</param>
        /// <param name="trayCode">关联托盘字典编码</param>
        /// <param name="warehouseTrayId">关联托盘字典编号</param>
        /// <param name="subOrderId">关联订单编号</param>
        /// <param name="subOrderRowId">关联订单行编号</param>
        /// <param name="carrier">所属载体</param>
        /// <param name="traySteps">关联托盘状态,多个以逗号隔开</param>
        /// <param name="locationId">所在货位编号</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="wareHouseId">所在仓库编号</param>
        /// <param name="areaId">所在分区编号</param>
        /// <param name="supplierId">供应商编号</param>
        /// <param name="supplierSiteId">供应商站编号</param>
        /// <param name="pyId">物理仓库编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMaterials(int? pageIndex,
            int? itemsPage, int? id, string materialCode, int? materialDicId, string trayCode,string materialName,string materialSpec,
            int? warehouseTrayId, int? subOrderId, int? subOrderRowId, int? carrier, string traySteps, int? locationId, int? ouId
            , int? wareHouseId, int? areaId,int? supplierId,int? supplierSiteId,int? pyId)
        {
            var response = await this._warehouseMaterialViewModelService.GetMaterials(pageIndex,
                itemsPage, id, materialCode, materialDicId, materialName,materialSpec,trayCode,warehouseTrayId, subOrderId,subOrderRowId,
                carrier, traySteps, locationId,ouId,wareHouseId, areaId,supplierId,supplierSiteId,pyId);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}
