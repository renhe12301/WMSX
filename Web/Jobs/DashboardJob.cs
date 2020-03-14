using System;
using System.Collections.Generic;
using System.Dynamic;
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
            try
            {
                await _hubContext.Clients.All.SendAsync("ShowTime", DateTime.Now.ToString());
                var inOutRecordChart = GetInOutRecordAnalysis();
                await _hubContext.Clients.All.SendAsync("ShowInOutRecordAnalysis", inOutRecordChart);
            }
            catch (Exception ex)
            {
                
            }
        }
        
        /// <summary>
        /// 出入库记录分析数据看板
        /// </summary>
        /// <returns></returns>
        List<List<int>> GetInOutRecordAnalysis()
        {
            List<List<int>> result = new List<List<int>>();
            List<int> data1 = new List<int>();
            List<int> data2 = new List<int>();
            List<int> data3 = new List<int>();
            List<int> data4 = new List<int>();
            Random random = new Random();
            for (int i = 1; i <=12; i++)
            {
                data1.Add(random.Next(10,100));
            }
            for (int i = 1; i <=12; i++)
            {
                data2.Add(random.Next(10,100));
            }
            for (int i = 1; i <=12; i++)
            {
                data3.Add(random.Next(10,100));
            }
            for (int i = 1; i <=12; i++)
            {
                data4.Add(random.Next(10,100));
            }
            result.Add(data1);
            result.Add(data2);
            result.Add(data3);
            result.Add(data4);
            return result;
        }
        
        

    }
    
}