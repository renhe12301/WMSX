using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels.StockManager;
using Web.ViewModels.TaskManager;

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
        /// 空托盘待入库任务申请
        /// </summary>
        /// <param name="warehouseTrayViewModel">仓库托盘实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EmptyAwaitInApply(WarehouseTrayViewModel warehouseTrayViewModel)
        {
            var response = await this._inOutTaskViewModelService.EmptyAwaitInApply(warehouseTrayViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 入库申请
        /// </summary>
        /// <param name="inOutTaskViewModel">出入库任务实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InApply(InOutTaskViewModel inOutTaskViewModel)
        {
            var response = await this._inOutTaskViewModelService.InApply(inOutTaskViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 待出库任务申请
        /// </summary>
        /// <param name="inOutTaskViewModel">出入库任务实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AwaitOutApply(InOutTaskViewModel inOutTaskViewModel)
        {
            var response = await this._inOutTaskViewModelService.AwaitOutApply(inOutTaskViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 出入库任务步骤上报
        /// </summary>
        /// <param name="inOutTaskViewModel">出入库任务实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TaskStepReport(InOutTaskViewModel inOutTaskViewModel)
        {
            var response = await this._inOutTaskViewModelService.TaskStepReport(inOutTaskViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页显示大小</param>
        /// <param name="id">任务编号</param>
        /// <param name="status">任务状态,多个以逗号隔开</param>
        /// <param name="steps">任务步骤,多个以逗号隔开</param>
        /// <param name="sCreateTime">任务开始时间</param>
        /// <param name="eCreateTIme">任务开始时间</param>
        /// <param name="sFinishTime"><任务完成时间</param>
        /// <param name="eFinishTime">任务完成时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInOutTasks(int? pageIndex, int? itemsPage,
                                             int? id, string status,string steps,
                                              int? orgId, int? ouId,
                                              int? wareHouseId, int? areaId,
                                              string sCreateTime, string eCreateTIme,
                                              string sFinishTime, string eFinishTime)
        {
            var response = await this._inOutTaskViewModelService.GetInOutTasks(pageIndex,
                itemsPage, id, status, steps,orgId,ouId,wareHouseId,areaId,sCreateTime, eCreateTIme, sFinishTime, eFinishTime);
            return Ok(response);
        }


    }
}
