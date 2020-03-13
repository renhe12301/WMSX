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

        private readonly IHubContext<ClockHub, IClock> _clockHub;

        private readonly IAsyncRepository<MaterialType> _materialTypeRepository;

        public DashboardJob()
        {
            _materialTypeRepository = EnginContext.Current.Resolve<IAsyncRepository<MaterialType>>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var materialDics = await this._materialTypeRepository.ListAllAsync();
            Console.WriteLine(materialDics.Count);
        }
    }
    
    public interface IClock
    {
        Task ShowTime(DateTime currentTime);
    }
    
    public class ClockHub : Hub<IClock>
    {
        public async Task SendTimeToClients(DateTime dateTime)
        {
            await Clients.All.ShowTime(dateTime);
        }
    }
}