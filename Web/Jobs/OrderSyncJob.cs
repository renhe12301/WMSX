using System;
using System.Threading.Tasks;
using Quartz;

namespace Web.Jobs
{
    /// <summary>
    /// 订单,订单行数据同步定时任务
    /// </summary>
    public class OrderStatusSyncJob:IJob
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