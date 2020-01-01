using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;

namespace Web.Controllers.Api
{
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
        /// <param name="type">出入库流程类型</param>
        /// <param name="inoutFlag">出入库标记</param>
        /// <param name="sCreateTime">创建开始时间</param>
        /// <param name="eCreateTime">创建结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInOutRecords(int? pageIndex,int? itemsPage,int? type,int? inoutFlag,
                                                        string sCreateTime,string eCreateTime)
        {
            var response = await this._inOutRecordViewModelService.GetInOutRecords(pageIndex,
                                       itemsPage, type, inoutFlag, sCreateTime, eCreateTime);
            return Ok(response);
        }
    }
}
