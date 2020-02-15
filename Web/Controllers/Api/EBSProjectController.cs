using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;

namespace Web.Controllers.Api
{
    [EnableCors("AllowCORS")]
    public class EBSProjectController:BaseApiController
    {
        private readonly IEBSProjectViewModelService _ebsProjectViewModelService;

        public EBSProjectController(IEBSProjectViewModelService ebsProjectViewModelService)
        {
            this._ebsProjectViewModelService = ebsProjectViewModelService;
        }
        
        /// <summary>
        /// 获取项目信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        /// <param name="id">编号</param>
        /// <param name="projectName">项目名称</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="sCreateTime">创建开始时间</param>
        /// <param name="eCreateTime">创建结束时间</param>
        /// <param name="sEndTime">有效期开始时间</param>
        /// <param name="eEndTime">有效期结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProjects(int? pageIndex,int? itemsPage, int? id,string projectName,int? ouId, 
            string sCreateTime, string eCreateTime,
            string sEndTime, string eEndTime)
        {
            var response  = await this._ebsProjectViewModelService.GetProjects(pageIndex,itemsPage,id,projectName,ouId,sCreateTime,
                eCreateTime,sEndTime,eEndTime);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}