using System;
using System.Threading.Tasks;
using Quartz;

namespace Web.Jobs
{
    public class Clear_Job: IJob
    {
        public Clear_Job()
        {
        }

        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
