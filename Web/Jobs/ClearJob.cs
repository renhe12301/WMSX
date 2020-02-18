using System;
using System.Threading.Tasks;
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

        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
