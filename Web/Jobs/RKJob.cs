using System;
using System.Threading.Tasks;
using ApplicationCore.Misc;
using Quartz;

namespace Web.Jobs
{
    /// <summary>
    /// 入库订单处理定时任务
    /// </summary>
    public class RKJob:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (ModuleLock.GetAsyncLock().LockAsync())
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
}