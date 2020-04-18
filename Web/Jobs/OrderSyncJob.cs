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
using Microsoft.EntityFrameworkCore;
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
        private readonly IAsyncRepository<OrderRowBatch> _orderRowBatchRepository;
        public OrderStatusSyncJob(IAsyncRepository<InOutRecord> inOutRecordRepository,
            IAsyncRepository<Order> orderRepository,
            IAsyncRepository<OrderRow> orderRowRepository,
            ILogRecordService logRecordService,
            IAsyncRepository<OrderRowBatch> orderRowBatchRepository
        )
        {
            this._inOutRecordRepository = inOutRecordRepository;
            this._orderRepository = orderRepository;
            this._orderRowRepository = orderRowRepository;
            this._logRecordService = logRecordService;
            this._orderRowBatchRepository = orderRowBatchRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                OrderSpecification orderSpec = new OrderSpecification(null, null, null,
                    new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)},null,null,null,null,null, null, null, null,
                    null, null, null, null, null, null, null,
                    null, null);
                List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
                OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null,null,null,null,
                    new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, null, null, null, null);
                List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                InOutRecordSpecification inOutRecordSpec = new InOutRecordSpecification(null, null, 
                    null, null, null,null, null, null, null,
                    null,new List<int> {Convert.ToInt32(ORDER_STATUS.完成)}, null, 0, null, null);
                List<InOutRecord> inOutRecords = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
                List<Order> updOrders = new List<Order>();
                List<OrderRow> updOrderRows = new List<OrderRow>();
                List<InOutRecord> updInOutRecords = new List<InOutRecord>();
                orders.ForEach(async (order) =>
                {
                    if (order.OrderTypeId != Convert.ToInt32(ORDER_TYPE.接收退料))
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
                                if ((or.RealityCount) >= (or.PreCount - or.BadCount - or.CancelCount))
                                    or.Status = Convert.ToInt32(ORDER_STATUS.完成);
                                updOrderRows.Add(or);
                            }
                        });
                        OrderRowSpecification allOrderRowSpec = new OrderRowSpecification(null, order.Id, null, null,
                            null,
                            null, null, null, null, null);
                        List<OrderRow> allOrderRows = await this._orderRowRepository.ListAsync(allOrderRowSpec);
                        int finishCount = allOrderRows.Count(or => or.Status == Convert.ToInt32(ORDER_STATUS.完成));
                        if (finishCount == allOrderRows.Count)
                        {
                            order.Status = Convert.ToInt32(ORDER_STATUS.完成);
                            updOrders.Add(order);
                        }
                    }
                });
                
                OrderRowBatchSpecification orderRowBatchSpec = new OrderRowBatchSpecification(null,null,
                    null,null,1,null,0,null);
                List<OrderRowBatch> orderRowBatchs = await this._orderRowBatchRepository.ListAsync(orderRowBatchSpec);
                List<OrderRowBatch> updOrderRowBatchs = new List<OrderRowBatch>();
                orderRowBatchs.ForEach(async (orb) =>
                {
                    InOutRecordSpecification childSpec = new InOutRecordSpecification(null,null,null,
                        null,null,null,null,null,orb.Id,
                        null,null,null,null,null,null );
                    List<InOutRecord> childInOutRecords = await this._inOutRecordRepository.ListAsync(childSpec);
                    List<InOutRecord> finishInOutRecords = childInOutRecords.Where(r => r.Status == Convert.ToInt32(ORDER_STATUS.完成)).ToList();
                    if (finishInOutRecords.Count() == childInOutRecords.Count())
                    {
                        orb.IsSync = 1;
                        orb.Status = Convert.ToInt32(ORDER_STATUS.完成);
                        updOrderRowBatchs.Add(orb);
                    }
                });

                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    this._orderRepository.Update(updOrders);
                    this._orderRowRepository.Update(updOrderRows);
                    this._inOutRecordRepository.Update(updInOutRecords);
                    this._orderRowBatchRepository.Update(updOrderRowBatchs);
                    scope.Complete();
                }

                StringBuilder logBuilder = new StringBuilder();
                if (updOrders.Count > 0)
                    logBuilder.Append(
                        string.Format("同步订单Id[{0}]状态！\n", string.Join(',', updOrders.ConvertAll(o => o.Id))));
                if (updOrderRows.Count > 0)
                    logBuilder.Append(string.Format("同步订单行Id[{0}]状态！\n",
                        string.Join(',', updOrderRows.ConvertAll(or => or.Id))));
                if (updOrderRowBatchs.Count > 0)
                    logBuilder.Append(string.Format("同步订单出库批次Id[{0}]状态！\n",
                        string.Join(',', updOrderRowBatchs.ConvertAll(orb => orb.Id))));
                if (inOutRecords.Count > 0)
                    logBuilder.Append(string.Format("同步订单行子行Id[{0}]状态！",
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