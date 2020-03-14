using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using log4net;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using Web;
using ApplicationCore.Specifications;

namespace Web.Jobs
{
    public class DashboardJob:IJob
    {
        private readonly IAsyncRepository<MaterialType> _materialTypeRepository;

        public DashboardJob()
        {
            _materialTypeRepository = EnginContext.Current.Resolve<IAsyncRepository<MaterialType>>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //await _clockHub.Clients.All.ShowTime("mm");
        }
    }
    
    public class ClockHub : Hub
    {
        public async Task SendTimeToClients(string msg)
        {
            await Clients.All.SendAsync("ShowTime","ssss");
        }
    }
}