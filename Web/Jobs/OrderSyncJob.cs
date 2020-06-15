using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Quartz;
using ApplicationCore.Misc;

namespace Web.Jobs
{
    /// <summary>
    /// 订单,订单行数据同步定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class OrderStatusSyncJob:IJob
    {
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        public OrderStatusSyncJob(IAsyncRepository<LogRecord> logRecordRepository,
                                  IAsyncRepository<SubOrder> subOrderRepository,
                                  IAsyncRepository<SubOrderRow> subOrderRowRepository)
        {
            this._logRecordRepository = logRecordRepository;
            this._subOrderRowRepository = subOrderRowRepository;
            this._subOrderRepository = subOrderRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {

                try
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        SubOrderRowSpecification subOrderRowSpec = new SubOrderRowSpecification(null, null,
                            null, null, null, null, null, null, null, null, null,
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

                        SubOrderSpecification subOrderSpecification = new SubOrderSpecification(null, null, null, null,
                            new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, null, null, null, null, null, null,
                            null, null, null, null, null, null, null);
                        List<SubOrder> subOrders = this._subOrderRepository.List(subOrderSpecification);
                        List<SubOrder> updSubOrders = new List<SubOrder>();
                        foreach (var subOrder in subOrders)
                        {
                            SubOrderRowSpecification allSubRowSpec = new SubOrderRowSpecification(null, subOrder.Id,
                                null, null, null, null, null, null, null, null, null,
                                null, null, null, null, null, null);
                            List<SubOrderRow> allRows = this._subOrderRowRepository.List(allSubRowSpec);

                            SubOrderRowSpecification endSubRowSpec = new SubOrderRowSpecification(null, subOrder.Id,
                                null, null, null, null, null, null, null, null, null,
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
                
            }
        }
    }
}