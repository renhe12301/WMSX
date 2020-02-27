using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 出入库记录API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class InOutRecordController:BaseApiController
    {
        private readonly IInOutRecordViewModelService _inOutRecordViewModelService;

        public InOutRecordController(IInOutRecordViewModelService inOutRecordViewModelService)
        {
            this._inOutRecordViewModelService = inOutRecordViewModelService;
        }

        /// <summary>
        /// 获取出入库记录
        /// </summary>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="itemsPage">当前也显示大小</param>
        /// <param name="trayCode">托盘编码</param>
        /// <param name="materialName">物料名称</param>
        /// <param name="type">出入库类型</param>
        /// <param name="orderId">订单编号</param>
        /// <param name="orderRowId">订单行编号</param>
        /// <param name="orderRowBatchId">订单行批次编号</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="wareHouseId">库存组织编号</param>
        /// <param name="areaId">库区编号</param>
        /// <param name="status">执行状态</param>
        /// <param name="sCreateTime">创建开始时间</param>
        /// <param name="eCreateTime">创建结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInOutRecords(int? pageIndex,int? itemsPage,string trayCode,string materialName,
                                                         int? type,int? orderId,int? orderRowId,int? orderRowBatchId,int? ouId,
                                                         int? wareHouseId, int? areaId,string status,
                                                         string sCreateTime, string eCreateTime)
        {
            var response = await this._inOutRecordViewModelService.GetInOutRecords(pageIndex,
                                       itemsPage, trayCode,materialName,type,ouId,wareHouseId,areaId,
                                       orderId,orderRowId,orderRowBatchId,status, sCreateTime, eCreateTime);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}
