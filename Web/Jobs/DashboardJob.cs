using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using ApplicationCore.Specifications;
using Web.Hubs;

namespace Web.Jobs
{
    [DisallowConcurrentExecution]
    public class DashboardJob : IJob
    {
        private readonly IAsyncRepository<WarehouseMaterial> _materialRepository;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
 
        public DashboardJob(IHubContext<DashboardHub> hubContext,
                            IAsyncRepository<WarehouseMaterial> materialRepository,
                            IAsyncRepository<Location> locationRepository,
                            IAsyncRepository<InOutTask> inOutTaskRepository,
                            IAsyncRepository<SubOrder> subOrderRepository)
        {
            _hubContext = hubContext;
            _materialRepository = materialRepository;
            _locationRepository = locationRepository;
            _inOutTaskRepository = inOutTaskRepository;
            _subOrderRepository = subOrderRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if ((DateTime.Now - DashboardHub.now).TotalSeconds < 3)
                {
                    await _hubContext.Clients.All.SendAsync("ShowTime", DateTime.Now.ToString());
                    var inRecordChart = await GetInRecordAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowInRecordAnalysis", inRecordChart);
                    var outRecordChart = await GetOutRecordAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowOutRecordAnalysis", outRecordChart);
                    var inOrderChart = await GetInOrderAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowInOrderAnalysis", inOrderChart);
                    var outOrderChart = await GetOutOrderAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowOutOrderAnalysis", outOrderChart);
                    var orderTypeChart = await GetOrderTypeAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowOrderTypeAnalysis", orderTypeChart);
                    var stockCountChart = await GetStockCountAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowStockCountAnalysis", stockCountChart);
                    var weekOrderChart = await GetWeekOrderAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowWeekOrderAnalysis", weekOrderChart);
                    var stockAssestChart = await GetStockAssestAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowStockAssestAnalysis", stockAssestChart);
                    var stockUtilizationChart = await GetStockUtilizationAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowStockUtilizationAnalysis", stockUtilizationChart);

                    var orderChronergyChart = await GetOrderChronergyAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowOrderChronergyChartAnalysis", orderChronergyChart);

                }
                else
                {
                    Console.WriteLine("大数据分析,等待客户端连接...");
                }

            }
            catch (Exception ex)
            {

            }
            
            Thread.Sleep(100);

        }

        /// <summary>
        /// 每月入库记录分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<List<double>>> GetInRecordAnalysis()
        {
            List<List<double>> result = new List<List<double>>();
            List<double> data1 = new List<double>();
            string ytime = DateTime.Now.Year.ToString();
           
            InOutTaskSpecification inOutTaskSpecification = new InOutTaskSpecification(null,null,null,null,
                null,null,null,null,null,null,null,null,
                null, ytime + "-01-01",ytime + "-12-31",null,null);
            List<InOutTask> inOutTasks = await this._inOutTaskRepository.ListAsync(inOutTaskSpecification);
           // Random random = new Random();
            for (int i = 1; i <= 12; i++)
            {
                string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                DateTime now = DateTime.Parse(time);
                var sCreateTime = now;
                var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                
                List<InOutTask> recvs = inOutTasks
                    .Where(r => r.Type == Convert.ToInt32(TASK_TYPE.物料入库) && r.CreateTime >= now &&
                                r.CreateTime <= eCreateTime).ToList();
                data1.Add(recvs.Count);
                //data1.Add(random.Next(10,100));
            }
            result.Add(data1);
            return result;
        }

        /// <summary>
        /// 每月出库记录分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<List<double>>> GetOutRecordAnalysis()
        {
            List<List<double>> result = new List<List<double>>();
            List<double> data1 = new List<double>();
            string ytime = DateTime.Now.Year.ToString();
           
            InOutTaskSpecification inOutTaskSpecification = new InOutTaskSpecification(null,null,null,null,
                null,null,null,null,null,null,null,null,
                null,ytime + "-01-01", ytime + "-12-31",null,null);
            List<InOutTask> inOutTasks = await this._inOutTaskRepository.ListAsync(inOutTaskSpecification);
            //Random random = new Random();
            for (int i = 1; i <= 12; i++)
            {
                string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                DateTime now = DateTime.Parse(time);
                var sCreateTime = now;
                var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                
                List<InOutTask> recvs = inOutTasks
                    .Where(r => r.Type == Convert.ToInt32(TASK_TYPE.物料出库) && r.CreateTime >= now &&
                                r.CreateTime <= eCreateTime).ToList();
                data1.Add(recvs.Sum(r => r.WarehouseTray.MaterialCount));
                //data1.Add(random.Next(10,100));
            }
            result.Add(data1);
            return result;
        }

        /// <summary>
        /// 每月订单完成时效分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<List<int>>> GetOrderChronergyAnalysis()
        {
            List<List<int>> result = new List<List<int>>();
            List<int> data1 = new List<int>();
            List<int> data2 = new List<int>();
            List<int> data3 = new List<int>();
            //Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();

            SubOrderSpecification subOrderSpec = new SubOrderSpecification(null,null,null,null,
                null,null,null,null,null,null,null,null,null,null,
                null,null, ytime + "-01-01",ytime + "-12-31",null,null);
            
            List<SubOrder> orders = await this._subOrderRepository.ListAsync(subOrderSpec);
            for (int i = 1; i <= 12; i++)
            {
                string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                DateTime now = DateTime.Parse(time);
                var sCreateTime = now;
                var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                List<SubOrder> orders2 = orders.Where(r =>
                    r.Status == Convert.ToInt32(ORDER_STATUS.待处理) && r.CreateTime >= now &&
                    r.CreateTime <= eCreateTime).ToList();
                List<SubOrder> orders3 = orders.Where(r =>
                    r.Status == Convert.ToInt32(ORDER_STATUS.执行中) && r.CreateTime >= now &&
                    r.CreateTime <= eCreateTime).ToList();
                List<SubOrder> orders4 = orders.Where(r =>
                    r.Status == Convert.ToInt32(ORDER_STATUS.完成) && r.CreateTime >= now &&
                    r.CreateTime <= eCreateTime).ToList();
                data1.Add(orders2.Count);
                data2.Add(orders3.Count);
                data3.Add(orders4.Count);
                // data1.Add(random.Next(10,100));
                // data2.Add(random.Next(10,100));
                // data3.Add(random.Next(10,100));
            }

            result.Add(data1);
            result.Add(data2);
            result.Add(data3);
            return result;
        }
        
        
        /// <summary>
        /// 每月入库单分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<List<int>>> GetInOrderAnalysis()
        {
            List<List<int>> result = new List<List<int>>();
            List<int> data1 = new List<int>();
            List<int> data2 = new List<int>();
            List<int> data3 = new List<int>();
            //Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            SubOrderSpecification subOrderSpec = new SubOrderSpecification(null,null,null,null,
                null,null,null,null,null,null,null,null,null,null,
                null,null, ytime + "-01-01",ytime + "-12-31",null,null);
            
            List<SubOrder> orders = await this._subOrderRepository.ListAsync(subOrderSpec);
            for (int i = 1; i <= 12; i++)
            {
                string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                DateTime now = DateTime.Parse(time);
                var sCreateTime = now;
                var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                List<SubOrder> orders2 = orders.Where(r =>
                    r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收) && r.CreateTime >= now &&
                    r.CreateTime <= eCreateTime).ToList();
               
                List<SubOrder> orders4 = orders.Where(r =>
                    r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退料) && r.CreateTime >= now &&
                    r.CreateTime <= eCreateTime).ToList();
                data1.Add(orders2.Count);
               
                data3.Add(orders4.Count);
                // data1.Add(random.Next(10,100));
                // data2.Add(random.Next(10,100));
                // data3.Add(random.Next(10,100));
            }

            result.Add(data1);
           // result.Add(data2);
            result.Add(data3);
            return result;
        }


        /// <summary>
        /// 每月出库单分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<List<int>>> GetOutOrderAnalysis()
        {
            List<List<int>> result = new List<List<int>>();
            List<int> data1 = new List<int>();
            List<int> data2 = new List<int>();
            //Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            SubOrderSpecification subOrderSpec = new SubOrderSpecification(null,null,null,null,
                null,null,null,null,null,null,null,null,null,null,
                null,null, ytime + "-01-01",ytime + "-12-31",null,null);
            
            List<SubOrder> orders = await this._subOrderRepository.ListAsync(subOrderSpec);
            for (int i = 1; i <= 12; i++)
            {
                string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                DateTime now = DateTime.Parse(time);
                var sCreateTime = now;
                var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                List<SubOrder> orders2 = orders.Where(r =>
                    r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料) && r.CreateTime >= now &&
                    r.CreateTime <= eCreateTime).ToList();
                List<SubOrder> orders3 = orders.Where(r =>
                    r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退库) && r.CreateTime >= now &&
                    r.CreateTime <= eCreateTime).ToList();
                data1.Add(orders2.Count);
                data2.Add(orders3.Count);
                // data1.Add(random.Next(10,100));
                // data2.Add(random.Next(10,100));
            }

            result.Add(data1);
            result.Add(data2);
            return result;
        }
        
        /// <summary>
        /// 订单类型分析
        /// </summary>
        /// <returns></returns>
        async Task<List<int>> GetOrderTypeAnalysis()
        {
            List<int> result = new List<int>();
            Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            SubOrderSpecification subOrderSpec = new SubOrderSpecification(null,null,null,null,
                null,null,null,null,null,null,null,null,
                null,null, null,null,null,null,null,null);
            List<SubOrder> orders = await this._subOrderRepository.ListAsync(subOrderSpec);
            List<SubOrder> orders2 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收)).ToList();
            List<SubOrder> orders3 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退库)).ToList();
            List<SubOrder> orders4 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退料)).ToList();
            List<SubOrder> orders5 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料)).ToList();
          
            result.Add(orders2.Count);
            result.Add(orders3.Count);
            result.Add(orders4.Count);
            result.Add(orders5.Count);
           
            return result;
        }
        
        /// <summary>
        /// 每月库存量分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<double>> GetStockCountAnalysis()
        {
            List<double> result = new List<double>();
            //Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null, null, null,
                null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, ytime + "-01-01", ytime + "-12-31");
            List<WarehouseMaterial> materials = await this._materialRepository.ListAsync(warehouseMaterialSpec);
            for (int i = 1; i <= 12; i++)
            {
                string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                DateTime now = DateTime.Parse(time);
                var sCreateTime = now;
                var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                
                List<WarehouseMaterial> ms = materials.Where(m=>m.CreateTime >= now &&m.CreateTime <= eCreateTime).ToList();
                result.Add(ms.Sum(r => r.MaterialCount));
                //result.Add(random.Next(10,100));
            }

            return result;
        }
        
        /// <summary>
        /// 每周订单分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<List<double>>> GetWeekOrderAnalysis()
        {
            List<List<double>> result = new List<List<double>>();
            List<double> data1 = new List<double>();
            List<double> data2 = new List<double>();
            List<double> data3 = new List<double>();
            List<double> data4 = new List<double>();
            List<double> data5 = new List<double>();
            //Random random = new Random();
            DateTime now  = DateTime.Now;
            
            int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
            dayWeek = dayWeek == 0 ? 7 : dayWeek;
            
            SubOrderSpecification subOrderSpec = new SubOrderSpecification(null,null,null,null,
                null,null,null,null,null,null,null,null,null,null,
                null,null, now.AddDays(-dayWeek).AddDays(1).ToString(), 
                now.AddDays(7 - (int)now.DayOfWeek).ToString(),null,null);
            List<SubOrder> orders = await this._subOrderRepository.ListAsync(subOrderSpec);
            
            for (int i = 1; i <= 7; i++)
            {
                var sCreateTime = now.AddDays(-dayWeek).AddDays(i).ToShortDateString() + " 00:00:00";
                var eCreateTime = now.AddDays(-dayWeek).AddDays(i).ToShortDateString() + " 23:59:59";
                
                
                List<SubOrder> orders2 = orders.Where(r =>
                    r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                    r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
                List<SubOrder> orders3 = orders.Where(r =>
                    r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退库) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                    r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
                List<SubOrder> orders4 = orders.Where(r =>
                    r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退料) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                    r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
                List<SubOrder> orders5= orders.Where(r =>
                    r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                    r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
             
                data1.Add(orders2.Count);
                data2.Add(orders3.Count);
                data3.Add(orders4.Count);
                data4.Add(orders5.Count);
                // data1.Add(random.Next(10,100));
                // data2.Add(random.Next(10,100));
                // data3.Add(random.Next(10,100));
                // data4.Add(random.Next(10,100));
                // data5.Add(random.Next(10,100));
            }

            result.Add(data1);
            result.Add(data2);
            result.Add(data3);
            result.Add(data4);
            result.Add(data5);
            return result;
        }
        
        /// <summary>
        /// 每月库存资产分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<double>> GetStockAssestAnalysis()
        {
            List<double> result = new List<double>();
            //Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null, null, null,
                null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, ytime + "-01-01", ytime + "-12-31");
            List<WarehouseMaterial> materials = await this._materialRepository.ListAsync(warehouseMaterialSpec);
            for (int i = 1; i <= 12; i++)
            {
                string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                DateTime now = DateTime.Parse(time);
                var sCreateTime = now;
                var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                
                List<WarehouseMaterial> ms = materials.Where(m=>m.CreateTime >= now &&m.CreateTime <= eCreateTime).ToList();
                result.Add(ms.Sum(r => r.Amount.GetValueOrDefault()));
                //result.Add(random.Next(10,100));
            }

            return result;
        }
        
        /// <summary>
        /// 库存利用率分析
        /// </summary>
        /// <returns></returns>
        async Task<List<double>> GetStockUtilizationAnalysis()
        {
            List<double> result = new List<double>();
            Random random = new Random();
            List<Location> all = await this._locationRepository.ListAllAsync();
            double emptyCount = all.Where(l => l.InStock == Convert.ToInt32(LOCATION_INSTOCK.无货)).Count();
            double emptyTrayCount = all.Where(l => l.InStock == Convert.ToInt32(LOCATION_INSTOCK.空托盘)).Count();
            double materialCount = all.Where(l => l.InStock == Convert.ToInt32(LOCATION_INSTOCK.有货)).Count();
            result.Add(all.Count>0?emptyCount/all.Count:0);
            result.Add(all.Count>0?emptyTrayCount/all.Count:0);
            result.Add(all.Count>0?materialCount/all.Count:0);
            return result;
        }
    }

}