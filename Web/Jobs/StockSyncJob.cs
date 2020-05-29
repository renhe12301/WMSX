using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;

namespace Web.Jobs
{
    /// <summary>
    /// 库存同步处理定时任务
    /// </summary>
    public class StockSyncJob: IJob
    {
        private readonly ILogRecordService _logRecordService;


        public StockSyncJob(ILogRecordService logRecordService)
        {
            this._logRecordService = logRecordService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
               

            }
            catch (Exception ex)
            {
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
        }
    }
    
    
    
}
