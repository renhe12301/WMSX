using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Quartz;
using ApplicationCore.Misc;
using Microsoft.EntityFrameworkCore;
using Web.Interfaces;
using Web.Services;

namespace Web.Jobs
{
    /// <summary>
    /// 订单,订单行数据同步定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class OrderStatusSyncJob:IJob
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        public OrderStatusSyncJob(IAsyncRepository<Order> orderRepository,
            IAsyncRepository<OrderRow> orderRowRepository,
            IAsyncRepository<LogRecord> logRecordRepository,
            IAsyncRepository<SubOrderRow> subOrderRowRepository,
            IAsyncRepository<WarehouseTray> warehouseTrayRepository,
            IAsyncRepository<SubOrder> subOrderRepository
        )
        {
            this._orderRepository = orderRepository;
            this._orderRowRepository = orderRowRepository;
            this._logRecordRepository = logRecordRepository;
            this._subOrderRowRepository = subOrderRowRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._subOrderRepository = subOrderRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {

                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        DateTime pre = DateTime.Now.AddMonths(-now.Month + 1).AddDays(-now.Day + 1);

                        //更新时间期限为一年以内的行处理的数量
                        OrderRowSpecification orderRowSpecification = new OrderRowSpecification(null, null,
                            null, null, null, null, pre.ToString(),
                            now.ToString(), null, null);

                        List<OrderRow> orderRows = this._orderRowRepository.List(orderRowSpecification);
                        List<OrderRow> updOrderRows = new List<OrderRow>();
                        foreach (var orderRow in orderRows)
                        {
                            SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(null, null,
                                orderRow.Id, null, null, null, null, null, null, null, null,
                                null, null, null, null, null, null);
                            List<SubOrderRow> subOrderRows = this._subOrderRowRepository.List(subOrderRowSpecification);
                            orderRow.Sorting = subOrderRows.Sum(r => r.Sorting);
                            orderRow.RealityCount = subOrderRows.Sum(r => r.RealityCount);
                            updOrderRows.Add(orderRow);
                        }

                        if (updOrderRows.Count > 0)
                            this._orderRowRepository.Update(updOrderRows);

                        SubOrderRowSpecification subOrderRowSpec = new SubOrderRowSpecification(null, null,
                            null, null, null, null, null, null, null, null, null,
                            null, new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, null, null, null, null);
                        List<SubOrderRow> subRows = this._subOrderRowRepository.List(subOrderRowSpec);
                        List<SubOrderRow> updSubRows = new List<SubOrderRow>();
                        foreach (var subRow in subRows)
                        {
                            WarehouseTraySpecification warehouseTraySpecification = new WarehouseTraySpecification(null,
                                null,
                                null, null, subRow.Id, null, null, null, null, null, null, null);
                            List<WarehouseTray> warehouseTrays =
                                this._warehouseTrayRepository.List(warehouseTraySpecification);
                            List<WarehouseTray> rukuTray =
                                warehouseTrays.Where(t => t.TrayStep == Convert.ToInt32(TRAY_STEP.入库完成)).ToList();
                            List<WarehouseTray> chukuTray = warehouseTrays
                                .Where(t => t.TrayStep == Convert.ToInt32(TRAY_STEP.出库完成等待确认)).ToList();
                            var taotalRukuTrayCnt = rukuTray.Sum(t => t.MaterialCount);
                            var totalChukuTrayCnt = chukuTray.Sum(t => t.OutCount);
                            subRow.RealityCount = taotalRukuTrayCnt > 0 ? taotalRukuTrayCnt : totalChukuTrayCnt;
                            if (subRow.RealityCount >= subRow.PreCount)
                                subRow.Status = Convert.ToInt32(ORDER_STATUS.完成);
                            updSubRows.Add(subRow);
                        }

                        if (updOrderRows.Count > 0)
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
                                subOrder.Status = Convert.ToInt32(ORDER_STATUS.完成);
                            updSubOrders.Add(subOrder);
                        }

                        if (updSubOrders.Count > 0)
                            this._subOrderRepository.Update(updSubOrders);

                        scope.Complete();

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
}