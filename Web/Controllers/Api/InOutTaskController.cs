﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;
using Web.ViewModels.StockManager;
using Web.ViewModels.TaskManager;
using Web.ViewModels.WCSManager;

namespace Web.Controllers.Api
{

    /// <summary>
    /// 出入库任务操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class InOutTaskController : BaseApiController
    {
        private readonly IInOutTaskViewModelService _inOutTaskViewModelService;

        public InOutTaskController(IInOutTaskViewModelService inOutTaskViewModelService)
        {
            this._inOutTaskViewModelService = inOutTaskViewModelService;
        }

        /// <summary>
        /// 空托盘入库标记
        /// </summary>
        /// <param name="warehouseTrayViewModel">仓库托盘实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EmptyEntry([FromBody]WarehouseTrayViewModel warehouseTrayViewModel)
        {
            var response = await this._inOutTaskViewModelService.EmptyEntry(warehouseTrayViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 输送线入库任务申请
        /// </summary>
        /// <param name="entryApplyViewModel">WCS仓库托盘实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EntryApply([FromBody]EntryApplyViewModel entryApplyViewModel)
        {
            var response = await this._inOutTaskViewModelService.EntryApply(entryApplyViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 托盘剩余物料入库
        /// </summary>
        /// <param name="warehouseTrayViewModel">仓库托盘实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TrayEntry([FromBody]WarehouseTrayViewModel warehouseTrayViewModel)
        {
            var response = await this._inOutTaskViewModelService.TrayEntry(warehouseTrayViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 空托盘出库任务申请
        /// </summary>
        /// <param name="warehouseTrayViewModel">仓库托盘实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EmptyOut([FromBody]WarehouseTrayViewModel warehouseTrayViewModel)
        {
            var response = await this._inOutTaskViewModelService.EmptyOut(warehouseTrayViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 出库完成待确认
        /// </summary>
        /// <param name="warehouseTrayViewModel">仓库托盘实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OutConfirm([FromBody]WarehouseTrayViewModel warehouseTrayViewModel)
        {
            var response = await this._inOutTaskViewModelService.OutConfirm(warehouseTrayViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 出入库任务步骤上报
        /// </summary>
        /// <param name="taskReportViewModel">WCS出入库任务实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TaskReport([FromBody]TaskReportViewModel taskReportViewModel)
        {
            var response = await this._inOutTaskViewModelService.TaskReport(taskReportViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页显示大小</param>
        /// <param name="id">任务编号</param>
        /// <param name="trayCode">托盘唯一编码</param>
        /// <param name="materialCode">托盘唯一编码</param>
        /// <param name="subOrderId">关联订单编号</param>
        ///  <param name="subOrderRowId">关联订单行编号</param>
        /// <param name="status">任务状态,多个以逗号隔开</param>
        /// <param name="steps">任务步骤,多个以逗号隔开</param>
        /// <param name="types">任务类型,多个以逗号隔开</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="wareHouseId">所属仓库编号</param>
        /// <param name="areaId">所属库区编号</param>
        /// <param name="pyId">物理仓库编号</param>
        /// <param name="sCreateTime">任务开始时间</param>
        /// <param name="eCreateTIme">任务开始时间</param>
        /// <param name="sFinishTime"><任务完成时间</param>
        /// <param name="eFinishTime">任务完成时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInOutTasks(int? pageIndex, int? itemsPage,
                                             int? id,string trayCode,string materialCode, int? subOrderId,int? subOrderRowId
                                             ,string status,string steps,string types,
                                             int? ouId, int? wareHouseId, int? areaId,int? pyId,
                                              string sCreateTime, string eCreateTIme,
                                              string sFinishTime, string eFinishTime)
        {
            var response = await this._inOutTaskViewModelService.GetInOutTasks(pageIndex,
                itemsPage, id,trayCode,materialCode,subOrderId,subOrderRowId,status, steps,types,ouId,wareHouseId,areaId,pyId,
                sCreateTime, eCreateTIme, sFinishTime, eFinishTime);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 手动发送wcs任务
        /// </summary>
        /// <param name="inOutTaskViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendWcs([FromBody]InOutTaskViewModel inOutTaskViewModel)
        {
            var response = await this._inOutTaskViewModelService.SendWcs(inOutTaskViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

    }
}
