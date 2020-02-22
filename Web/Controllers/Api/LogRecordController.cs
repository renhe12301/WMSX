using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 日志信息操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class LogRecordController:BaseApiController
    {
        private readonly ILogRecordViewModelService _logRecordViewModelService;

        public LogRecordController(ILogRecordViewModelService logRecordViewModelService)
        {
            this._logRecordViewModelService = logRecordViewModelService;
        }

        /// <summary>
        /// 获取日志信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">页面显示大小</param>
        /// <param name="logType">日志类型</param>
        /// <param name="logDesc">日志内容</param>
        /// <param name="founder">日志记录者</param>
        /// <param name="sCreateTime">日志创建开始时间</param>
        /// <param name="eCreateTIme">日志创建结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLogRecords(int? pageIndex, int? itemsPage, int? logType,
            string logDesc,string founder,string sCreateTime, string eCreateTIme)
        {
            var response = await this._logRecordViewModelService.GetLogRecords(pageIndex, itemsPage, logType, logDesc,
                founder,sCreateTime, eCreateTIme);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}