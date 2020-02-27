using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Quartz;
using ApplicationCore.Misc;
using Web.Interfaces;

namespace Web.Jobs
{
    /// <summary>
    /// 订单,订单行数据同步定时任务
    /// </summary>
    public class OrderStatusSyncJob:IJob
    {
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRepository;
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly ILogRecordService _logRecordService;
        public OrderStatusSyncJob(IAsyncRepository<InOutRecord> inOutRecordRepository,
            IAsyncRepository<Order> orderRepository,
            IAsyncRepository<OrderRow> orderRowRepository,
            ILogRecordService logRecordService
        )
        {
            this._inOutRecordRepository = inOutRecordRepository;
            this._orderRepository = orderRepository;
            this._orderRowRepository = orderRowRepository;
            this._logRecordService = logRecordService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                OrderSpecification orderSpec = new OrderSpecification(null, null, null,
                    new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, null, null, null,
                    null, null, null, null, null, null, null,
                    null, null);
                List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
                OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null,
                    new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, null, null, null, null);
                List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                InOutRecordSpecification inOutRecordSpec = new InOutRecordSpecification(null, null, 
                    null, null, null,null, null, null, null,
                    new List<int> {Convert.ToInt32(ORDER_STATUS.完成)}, null, 0, null, null);
                List<InOutRecord> inOutRecords = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
                List<Order> updOrders = new List<Order>();
                List<OrderRow> updOrderRows = new List<OrderRow>();
                List<InOutRecord> updInOutRecords = new List<InOutRecord>();
                orders.ForEach(async (order) =>
                {
                    List<OrderRow> ors = orderRows.Where(or => or.OrderId == order.Id).ToList();
                    ors.ForEach(or =>
                    {
                        List<InOutRecord> rowRecords = inOutRecords.Where(r => r.OrderRowId == or.Id).ToList();
                        if (rowRecords.Count > 0)
                        {
                            or.RealityCount += rowRecords.Sum(r => r.InOutCount);
                            rowRecords.ForEach(r => r.IsSync = 1);
                            updInOutRecords.AddRange(rowRecords);
                            if (or.RealityCount >= or.PreCount)
                                or.Status = Convert.ToInt32(ORDER_STATUS.完成);
                            updOrderRows.Add(or);
                        }
                    });
                    OrderRowSpecification allOrderRowSpec = new OrderRowSpecification(null, order.Id,
                        null, null, null, null, null);
                    List<OrderRow> allOrderRows = await this._orderRowRepository.ListAsync(allOrderRowSpec);
                    int finishCount = allOrderRows.Count(or => or.Status == Convert.ToInt32(ORDER_STATUS.完成));
                    if (finishCount == allOrderRows.Count)
                    {
                        order.Status = Convert.ToInt32(ORDER_STATUS.完成);
                        updOrders.Add(order);
                    }
                });
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    this._orderRepository.Update(updOrders);
                    this._orderRowRepository.Update(updOrderRows);
                    this._inOutRecordRepository.Update(updInOutRecords);
                    scope.Complete();
                }

                StringBuilder logBuilder = new StringBuilder();
                if (updOrders.Count > 0)
                    logBuilder.Append(
                        string.Format("同步订单[{0}]状态！\n", string.Join(',', updOrders.ConvertAll(o => o.Id))));
                if (updOrderRows.Count > 0)
                    logBuilder.Append(string.Format("同步订单行[{0}]状态！\n",
                        string.Join(',', updOrderRows.ConvertAll(or => or.Id))));
                if (inOutRecords.Count > 0)
                    logBuilder.Append(string.Format("同步订单行子行[{0}]状态！",
                        string.Join(',', inOutRecords.ConvertAll(ior => ior.Id))));

                string record = logBuilder.ToString();
                if (!string.IsNullOrEmpty(record))
                {
                    await this._logRecordService.AddLog(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.定时任务日志),
                        LogDesc = record,
                        CreateTime = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
        }
    }
}