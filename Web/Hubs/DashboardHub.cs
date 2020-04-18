using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class DashboardHub: Hub
    {

        public static DateTime now = DateTime.Now.AddSeconds(-10);

        public async Task SendTimeToClients()
        {
            await Clients.All.SendAsync("ShowTime",DateTime.Now.ToString());
        }

        public async Task SendHeart()
        {
            now = DateTime.Now;
        }
    }
}