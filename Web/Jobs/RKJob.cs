using System.Threading.Tasks;
using Quartz;

namespace Web.Jobs
{
    /// <summary>
    /// 入库订单处理定时任务
    /// </summary>
    public class RKJob:IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}