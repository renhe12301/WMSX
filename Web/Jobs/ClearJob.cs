using System;
using System.Threading.Tasks;
using Quartz;

namespace Web.Jobs
{
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
