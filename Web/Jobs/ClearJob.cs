using System;
using System.Threading.Tasks;
using ApplicationCore.Misc;
using Quartz;

namespace Web.Jobs
{
    /// <summary>
    /// 清理系统日志定时任务
    /// </summary>
    public class ClearJob: IJob
    {
        public ClearJob()
        {
        }

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
