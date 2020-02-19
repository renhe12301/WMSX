using System;
using System.Threading.Tasks;
using Quartz;

namespace Web.Jobs
{
    /// <summary>
    /// 主数据同步定时任务
    /// </summary>
    public class BasicDataSyncJob:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                    
            }
            catch (Exception ex)
            {
                   
            }
        }
    }
}