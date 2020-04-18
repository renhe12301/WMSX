using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.StockManager;
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
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRepository;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;

        public DashboardJob(IHubContext<DashboardHub> hubContext)
        {
            _hubContext = hubContext;
            _materialRepository = EnginContext.Current.Resolve<IAsyncRepository<WarehouseMaterial>>();
            _inOutRecordRepository = EnginContext.Current.Resolve<IAsyncRepository<InOutRecord>>();
            _orderRepository = EnginContext.Current.Resolve<IAsyncRepository<Order>>();
            _locationRepository = EnginContext.Current.Resolve<IAsyncRepository<Location>>();
            _warehouseTrayRepository = EnginContext.Current.Resolve<IAsyncRepository<WarehouseTray>>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if ((DateTime.Now - DashboardHub.now).TotalSeconds < 3)
                {
                    await _hubContext.Clients.All.SendAsync("ShowTime", DateTime.Now.ToString());
                    var inOutRecordChart = await GetInOutRecordAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowInOutRecordAnalysis", inOutRecordChart);
                    var inBadChart = await GetInBadAnalysis();
                    await _hubContext.Clients.All.SendAsync("ShowInBadAnalysis", inBadChart);
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
                }
                else 
                {
                    Console.WriteLine("等待客户端连接...");
                }
                
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 每月出入库记录分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<List<int>>> GetInOutRecordAnalysis()
        {
            List<List<int>> result = new List<List<int>>();
            List<int> data1 = new List<int>();
            List<int> data2 = new List<int>();
            List<int> data3 = new List<int>();
            List<int> data4 = new List<int>();
            string ytime = DateTime.Now.Year.ToString();
            InOutRecordSpecification inOutRecordSpec = new InOutRecordSpecification(null, null, null,
                null, null, null, null, null, null, null, null, null,
                null, ytime + "-01-01", ytime + "-12-31");
            List<InOutRecord> inOutRecords = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
            // 接收
            Random random = new Random();
            for (int i = 1; i <= 12; i++)
            {
                // string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                // DateTime now = DateTime.Parse(time);
                // var sCreateTime = now;
                // var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                //
                // List<InOutRecord> recvs = inOutRecords
                //     .Where(r => r.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收) && r.CreateTime >= now &&
                //                 r.CreateTime <= eCreateTime).ToList();
                // data1.Add(recvs.Sum(r => r.InOutCount));
                //
                // // 退料
                // List<InOutRecord> tls = inOutRecords
                //     .Where(r => r.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退料) && r.CreateTime >= now &&
                //                 r.CreateTime <= eCreateTime).ToList();
                // data2.Add(tls.Sum(r => r.InOutCount));
                //
                // // 领料
                // List<InOutRecord> lls = inOutRecords
                //     .Where(r => r.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退料) && r.CreateTime >= now &&
                //                 r.CreateTime <= eCreateTime).ToList();
                // data3.Add(lls.Sum(r => r.InOutCount));
                //
                // // 退库
                // List<InOutRecord> tks = inOutRecords
                //     .Where(r => r.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库) && r.CreateTime >= now &&
                //                 r.CreateTime <= eCreateTime).ToList();
                // data4.Add(tks.Sum(r => r.InOutCount));
                data1.Add(random.Next(10,100));
                data2.Add(random.Next(10,100));
                data3.Add(random.Next(10,100));
                data4.Add(random.Next(10,100));
            }

            result.Add(data1);
            result.Add(data2);
            result.Add(data3);
            result.Add(data4);
            return result;
        }

        /// <summary>
        /// 每月入库不合格分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<int>> GetInBadAnalysis()
        {
            List<int> result = new List<int>();
            Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            InOutRecordSpecification inOutRecordSpec = new InOutRecordSpecification(null, null, null,
                null, null, null, null, null, null, null, null, null,
                null, ytime + "-01-01", ytime + "-12-31");
            List<InOutRecord> inOutRecords = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
            for (int i = 1; i <= 12; i++)
            {
                // string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                // DateTime now = DateTime.Parse(time);
                // var sCreateTime = now;
                // var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                //
                // List<InOutRecord> bads = inOutRecords
                //     .Where(r => r.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收)&&r.CreateTime >= now &&
                //                 r.CreateTime <= eCreateTime).ToList();
                // result.Add(bads.Sum(r => r.BadCount.GetValueOrDefault()));
                result.Add(random.Next(10,100));
            }

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
            Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            OrderSpecification orderSpec = new OrderSpecification(null, null, null, null, null,
                null, null, null, null, null, null, null,
                null, null, null,null, null, ytime + "-01-01", ytime + "-12-31", null, null);
            List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
            for (int i = 1; i <= 12; i++)
            {
                // string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                // DateTime now = DateTime.Parse(time);
                // var sCreateTime = now;
                // var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                // List<Order> orders2 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收) && r.CreateTime >= now &&
                //     r.CreateTime <= eCreateTime).ToList();
                // List<Order> orders3 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.接收退料) && r.CreateTime >= now &&
                //     r.CreateTime <= eCreateTime).ToList();
                // List<Order> orders4 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退料) && r.CreateTime >= now &&
                //     r.CreateTime <= eCreateTime).ToList();
                // data1.Add(orders2.Count);
                // data2.Add(orders3.Count);
                // data3.Add(orders4.Count);
                data1.Add(random.Next(10,100));
                data2.Add(random.Next(10,100));
                data3.Add(random.Next(10,100));
            }

            result.Add(data1);
            result.Add(data2);
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
            Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            OrderSpecification orderSpec = new OrderSpecification(null, null, null, null, null,
                null, null, null, null, null, null, null,
                null, null, null,null ,null , ytime + "-01-01", ytime + "-12-31",
                null, null);
            List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
            for (int i = 1; i <= 12; i++)
            {
                // string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                // DateTime now = DateTime.Parse(time);
                // var sCreateTime = now;
                // var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                // List<Order> orders2 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料) && r.CreateTime >= now &&
                //     r.CreateTime <= eCreateTime).ToList();
                // List<Order> orders3 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库) && r.CreateTime >= now &&
                //     r.CreateTime <= eCreateTime).ToList();
                // data1.Add(orders2.Count);
                // data2.Add(orders3.Count);
                data1.Add(random.Next(10,100));
                data2.Add(random.Next(10,100));
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
            OrderSpecification orderSpec = new OrderSpecification(null, null, null, null, null,
                null, null, null, null, null, null, null,
                null, null, null,null ,null , ytime + "-01-01", ytime + "-12-31",
                null, null);
            List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
            List<Order> orders2 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收)).ToList();
            List<Order> orders3 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库)).ToList();
            List<Order> orders4 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退料)).ToList();
            List<Order> orders5 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料)).ToList();
            List<Order> orders6 = orders.Where(r =>
                r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.接收退料)).ToList();
            result.Add(orders2.Count);
            result.Add(orders3.Count);
            result.Add(orders4.Count);
            result.Add(orders5.Count);
            result.Add(orders6.Count);
            return result;
        }
        
        /// <summary>
        /// 每月库存量分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<int>> GetStockCountAnalysis()
        {
            List<int> result = new List<int>();
            Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null, null, null,
                null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, ytime + "-01-01", ytime + "-12-31");
            List<WarehouseMaterial> materials = await this._materialRepository.ListAsync(warehouseMaterialSpec);
            for (int i = 1; i <= 12; i++)
            {
                // string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                // DateTime now = DateTime.Parse(time);
                // var sCreateTime = now;
                // var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                //
                // List<WarehouseMaterial> ms = materials.Where(m=>m.CreateTime >= now &&m.CreateTime <= eCreateTime).ToList();
                // result.Add(ms.Sum(r => r.MaterialCount));
                result.Add(random.Next(10,100));
            }

            return result;
        }
        
        /// <summary>
        /// 每周订单分析数据看板
        /// </summary>
        /// <returns></returns>
        async Task<List<List<int>>> GetWeekOrderAnalysis()
        {
            List<List<int>> result = new List<List<int>>();
            List<int> data1 = new List<int>();
            List<int> data2 = new List<int>();
            List<int> data3 = new List<int>();
            List<int> data4 = new List<int>();
            List<int> data5 = new List<int>();
            Random random = new Random();
            DateTime now  = DateTime.Now;
            OrderSpecification orderSpec = new OrderSpecification(null, null, null, null, null,
                null, null, null,null,null, null, null, null, null,
                null, null, null, now.AddDays(-(int)now.DayOfWeek + 1).ToString(), 
                now.AddDays(7 - (int)now.DayOfWeek).ToString(), null, null);
            List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
            for (int i = 1; i <= 7; i++)
            {
                // var sCreateTime = now.AddDays(-(int) now.DayOfWeek + i).ToString() + "00:00:00";
                // var eCreateTime = now.AddDays(-(int) now.DayOfWeek + i).ToString() + "23:59:59";
                // List<Order> orders2 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                //     r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
                // List<Order> orders3 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                //     r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
                // List<Order> orders4 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退料) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                //     r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
                // List<Order> orders5= orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                //     r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
                // List<Order> orders6 = orders.Where(r =>
                //     r.OrderTypeId == Convert.ToInt32(ORDER_TYPE.接收退料) && r.CreateTime >= DateTime.Parse(sCreateTime) &&
                //     r.CreateTime <= DateTime.Parse(eCreateTime)).ToList();
                // data1.Add(orders2.Count);
                // data2.Add(orders3.Count);
                // data3.Add(orders4.Count);
                // data4.Add(orders5.Count);
                // data5.Add(orders6.Count);
                data1.Add(random.Next(10,100));
                data2.Add(random.Next(10,100));
                data3.Add(random.Next(10,100));
                data4.Add(random.Next(10,100));
                data5.Add(random.Next(10,100));
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
            Random random = new Random();
            string ytime = DateTime.Now.Year.ToString();
            WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null, null, null,
                null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, ytime + "-01-01", ytime + "-12-31");
            List<WarehouseMaterial> materials = await this._materialRepository.ListAsync(warehouseMaterialSpec);
            for (int i = 1; i <= 12; i++)
            {
                // string time = DateTime.Now.Year + "-" + i.ToString().PadLeft(2, '0') + "-01";
                // DateTime now = DateTime.Parse(time);
                // var sCreateTime = now;
                // var eCreateTime = now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                //
                // List<WarehouseMaterial> ms = materials.Where(m=>m.CreateTime >= now &&m.CreateTime <= eCreateTime).ToList();
                // result.Add(ms.Sum(r => r.Price.GetValueOrDefault()));
                result.Add(random.Next(10,100));
            }

            return result;
        }
        
        /// <summary>
        /// 库存利用率分析
        /// </summary>
        /// <returns></returns>
        async Task<List<int>> GetStockUtilizationAnalysis()
        {
            List<int> result = new List<int>();
            Random random = new Random();
            List<Location> all = await this._locationRepository.ListAllAsync();
            int emptyCount = all.Where(l => l.InStock == Convert.ToInt32(LOCATION_INSTOCK.无货)).Count();
            int emptyTrayCount = all.Where(l => l.InStock == Convert.ToInt32(LOCATION_INSTOCK.空托盘)).Count();
            int materialCount = all.Where(l => l.InStock == Convert.ToInt32(LOCATION_INSTOCK.有货)).Count();
            result.Add(all.Count>0?emptyCount/all.Count:0);
            result.Add(all.Count>0?emptyTrayCount/all.Count:0);
            result.Add(all.Count>0?materialCount/all.Count:0);
            return result;
        }
    }

}