using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 日志信息操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class LogRecordController
    {
        private readonly ILogRecordViewModelService _logRecordViewModelService;

        public LogRecordController(ILogRecordViewModelService logRecordViewModelService)
        {
            this._logRecordViewModelService = logRecordViewModelService;
        }

        /// <summary>
        /// 获取日志信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="itemsPage"></param>
        /// <param name="logType"></param>
        /// <param name="logDesc"></param>
        /// <param name="sCreateTime"></param>
        /// <param name="eCreateTIme"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResultViewModel> GetLogRecords(int? pageIndex, int? itemsPage, int? logType,
            string logDesc,string sCreateTime, string eCreateTIme)
        {
            var response = await this._logRecordViewModelService.GetLogRecords(pageIndex, itemsPage, logType, logDesc,
                sCreateTime, eCreateTIme);
            return response;
        }
    }
}