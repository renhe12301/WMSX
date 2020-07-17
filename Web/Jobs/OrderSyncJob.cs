using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Quartz;
using ApplicationCore.Misc;
using System.Linq;

namespace Web.Jobs
{
    /// <summary>
    /// 订单,订单行数据同步定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class OrderStatusSyncJob:IJob
    {
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        public OrderStatusSyncJob(IAsyncRepository<LogRecord> logRecordRepository,
                                  IAsyncRepository<Order> orderRepository,
                                  IAsyncRepository<OrderRow> orderRowRepository,
                                  IAsyncRepository<SubOrder> subOrderRepository,
                                  IAsyncRepository<SubOrderRow> subOrderRowRepository)
        {
            this._logRecordRepository = logRecordRepository;
            this._subOrderRowRepository = subOrderRowRepository;
            this._subOrderRepository = subOrderRepository;
            this._orderRepository = orderRepository;
            this._orderRowRepository = orderRowRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    // 更新后置订单状态
                    SubOrderRowSpecification subOrderRowSpec = new SubOrderRowSpecification(null, null,
                        null, null, null, null, null, null,null, null, null, null,null,null,
                        null, new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, null, null, null, null);
                    List<SubOrderRow> subRows = this._subOrderRowRepository.List(subOrderRowSpec);
                    List<SubOrderRow> updSubRows = new List<SubOrderRow>();
                    foreach (var subRow in subRows)
                    {
                        if (subRow.RealityCount >= subRow.PreCount)
                        {
                            subRow.Status = Convert.ToInt32(ORDER_STATUS.完成);
                            updSubRows.Add(subRow);
                        }
                    }

                    if (updSubRows.Count > 0)
                        this._subOrderRowRepository.Update(updSubRows);

                    SubOrderSpecification subOrderSpecification = new SubOrderSpecification(null, null, null, null,null,
                        new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, null, null,null, null, null, null, null,
                        null, null, null, null, null, null, null);
                    List<SubOrder> subOrders = this._subOrderRepository.List(subOrderSpecification);
                    List<SubOrder> updSubOrders = new List<SubOrder>();
                    foreach (var subOrder in subOrders)
                    {
                        SubOrderRowSpecification allSubRowSpec = new SubOrderRowSpecification(null, subOrder.Id,
                            null, null, null, null, null, null, null, null, null,null,null,null,
                            null, null, null, null, null, null);
                        List<SubOrderRow> allRows = this._subOrderRowRepository.List(allSubRowSpec);

                        SubOrderRowSpecification endSubRowSpec = new SubOrderRowSpecification(null, subOrder.Id,
                            null, null, null, null, null, null, null, null, null,null,null,null,
                            null, new List<int> {Convert.ToInt32(ORDER_STATUS.完成)}, null, null, null, null);
                        List<SubOrderRow> endRows = this._subOrderRowRepository.List(endSubRowSpec);
                        if (allRows.Count == endRows.Count)
                        {
                            subOrder.Status = Convert.ToInt32(ORDER_STATUS.完成);
                            updSubOrders.Add(subOrder);
                        }
                    }

                    if (updSubOrders.Count > 0)
                        this._subOrderRepository.Update(updSubOrders);



                    // 更新前置订单状态
                    OrderRowSpecification orderRowSpec = new OrderRowSpecification(null, null, null, null, null, null, null, null, null, null, null,
                        null, null, null, new List<int> { Convert.ToInt32(ORDER_STATUS.执行中) }, null, null, null, null);
                    List<OrderRow> exeOrderRows = this._orderRowRepository.List(orderRowSpec);
                    List<OrderRow> updOrderRows = new List<OrderRow>();
                    foreach (var orderRow in exeOrderRows)
                    {
                        SubOrderRowSpecification curSubOrderRowSpec = new SubOrderRowSpecification(null, null, orderRow.Id, null, null, 
                                                                                                null, null, null, null, null, null, null, null, null,
                        null, new List<int> { Convert.ToInt32(ORDER_STATUS.完成) }, null, null, null, null);
                        List<SubOrderRow> curSubRows = this._subOrderRowRepository.List(curSubOrderRowSpec);
                        double subRowCountTotal = curSubRows.Sum(s => s.PreCount);
                        // 接收退库的数量
                        OrderRowRelatedSpecification jstlOrderRowSpecification = new OrderRowRelatedSpecification(
                           new List<int> { Convert.ToInt32(ORDER_TYPE.接收退料) }, orderRow.SourceId);
                        List<OrderRow> jstlOrderRows = this._orderRowRepository.List(jstlOrderRowSpecification);
                        double jstkCount = jstlOrderRows.Sum(t => t.PreCount);
                        double orderRowExpendCount = (orderRow.PreCount - jstkCount);

                        if (subRowCountTotal >= orderRowExpendCount)
                        {
                            orderRow.Status = Convert.ToInt32(ORDER_STATUS.完成);
                            updOrderRows.Add(orderRow);
                        }
                    }
                    if (updOrderRows.Count > 0)
                        this._orderRowRepository.Update(updOrderRows);

                    OrderSpecification orderSpec = new OrderSpecification(null, null, null, null, new List<int> { Convert.ToInt32(ORDER_STATUS.执行中) }, null,
                        null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                    List<Order> exeOrders = this._orderRepository.List(orderSpec);
                    List<Order> updOrders = new List<Order>();
                    foreach (var order in exeOrders) 
                    {
                        OrderRowSpecification curOrderRowSpec = new OrderRowSpecification(null, order.Id, null, null, null, null, null, null, null, null, null,
                        null, null, null, new List<int> { Convert.ToInt32(ORDER_STATUS.完成) }, null, null, null, null);

                        List<OrderRow> curOrderRows = this._orderRowRepository.List(curOrderRowSpec);

                        OrderRowSpecification allOrderRowSpec = new OrderRowSpecification(null, order.Id, null, null, null, null, null, null, null, null, null,
                       null, null, null, null, null, null, null, null);

                        List<OrderRow> allOrderRows = this._orderRowRepository.List(allOrderRowSpec);

                        if (curOrderRows.Count == allOrderRows.Count)
                        {
                            order.Status = Convert.ToInt32(ORDER_STATUS.完成);
                            updOrders.Add(order);
                        }

                    }
                    if (updOrders.Count > 0)
                        this._orderRepository.Update(updOrders);

                    scope.Complete();

                }

            }
            catch (Exception ex)
            {
                this._logRecordRepository.Add(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.Message,
                    CreateTime = DateTime.Now
                });
            }

            Thread.Sleep(100);
        }
    }
}