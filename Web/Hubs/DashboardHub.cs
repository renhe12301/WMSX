using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class DashboardHub: Hub
    {
        public async Task SendTimeToClients()
        {
            await Clients.All.SendAsync("ShowTime",DateTime.Now.ToString());
        }
    }
}