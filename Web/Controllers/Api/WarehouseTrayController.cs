using System;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 仓库信息操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class WarehouseTrayController:BaseApiController
    {
        private readonly IWarehouseTrayViewModelService _warehouseTrayViewModelService;
        public WarehouseTrayController(IWarehouseTrayViewModelService warehouseTrayViewModelService)
        {
            this._warehouseTrayViewModelService = warehouseTrayViewModelService;
        }


        /// <summary>
        /// 获取库区托盘数据
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">一页显示大小</param>
        /// <param name="id">托盘唯一id</param>
        /// <param name="trayCode">托盘唯一编码</param>
        /// <param name="rangeMaterialCount">托盘物料数量范围,例如 1,100/param>
        /// <param name="orderId">关联订单编号</param>
        /// <param name="orderRowId">关联订单行编号</param>
        /// <param name="carrier">载体</param>
        /// <param name="trayTaskStatus">托盘任务状态,多个以逗号隔开</param>
        /// <param name="locationId">所在货位编号</param>
        /// <param name="wareHouseId">所在仓库编号</param>
        /// <param name="areaId">所在库区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTrays(int? pageIndex, int? itemsPage,
              int? id, string trayCode, string rangeMaterialCount,
             int? orderId, int? orderRowId, int? carrier,
            string trayTaskStatus, int? locationId,int? ouId, int? wareHouseId, int? areaId)
        {
            var response = await this._warehouseTrayViewModelService.GetTrays(pageIndex,itemsPage,id,
                trayCode,rangeMaterialCount, orderId,orderRowId,
                carrier, trayTaskStatus, locationId,ouId, wareHouseId, areaId);
            return Content(JsonConvert.SerializeObject(response));
        }

    }
}
