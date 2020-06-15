using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Quartz;

namespace Web.Jobs
{
    /// <summary>
    /// 清理系统日志定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class ClearJob: IJob
    {
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        
        public ClearJob(IAsyncRepository<LogRecord> logRecordRepository)
        {
            this._logRecordRepository = logRecordRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                   DateTime ptime = DateTime.Now.AddDays(-7);
                   DateTime pptime = ptime.AddDays(-7);
                   LogRecordSpecification logRecordSpec = new LogRecordSpecification(null,null,null,pptime.ToString(),ptime.ToString());
                   List<LogRecord> logRecords = await this._logRecordRepository.ListAsync(logRecordSpec);
                   if (logRecords.Count > 0)
                       await this._logRecordRepository.DeleteAsync(logRecords);
                }
                catch (Exception ex)
                {
                         
                }
            }
            
            Thread.Sleep(100);
        }
    }
}
