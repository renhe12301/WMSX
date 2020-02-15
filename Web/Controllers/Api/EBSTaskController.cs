using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;

namespace Web.Controllers.Api
{
    [EnableCors("AllowCORS")]
    public class EBSTaskController:BaseApiController
    {
        private readonly IEBSTaskViewModelService _ebsTaskViewModelService;

        public EBSTaskController(IEBSTaskViewModelService ebsTaskViewModelService)
        {
            this._ebsTaskViewModelService = ebsTaskViewModelService;
        }
        
        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        /// <param name="id">编号</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="projectId">关联项目编号</param>
        /// <param name="sCreateTime">创建开始时间</param>
        /// <param name="eCreateTime">创建结束时间</param>
        /// <param name="sEndTime">有效期开始时间</param>
        /// <param name="eEndTime">有效期结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTasks(int? pageIndex, int? itemsPage,int? id,string taskName,int? projectId, 
            string sCreateTime, string eCreateTime,
            string sEndTime, string eEndTime)
        {
            var response  = await this._ebsTaskViewModelService.GetTasks(pageIndex,itemsPage,id,taskName,projectId,sCreateTime,
                eCreateTime,sEndTime,eEndTime);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}