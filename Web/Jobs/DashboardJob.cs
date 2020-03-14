using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using log4net;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using Web;
using ApplicationCore.Specifications;
using Web.Hubs;

namespace Web.Jobs
{
    public class DashboardJob:IJob
    {
        private readonly IAsyncRepository<MaterialType> _materialTypeRepository;
        private readonly IHubContext<DashboardHub> _hubContext;
        public DashboardJob(IHubContext<DashboardHub> hubContext)
        {
            _hubContext = hubContext;
            _materialTypeRepository = EnginContext.Current.Resolve<IAsyncRepository<MaterialType>>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _hubContext.Clients.All.SendAsync("ShowTime", DateTime.Now.ToString());
        }
    }
    
}