using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class SubOrderService:ISubOrderService
    {

        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;

        public SubOrderService(IAsyncRepository<SubOrder> subOrderRepository,
                               IAsyncRepository<SubOrderRow> subOrderRowRepository,
                               IAsyncRepository<LogRecord> logRecordRepository,
                               IAsyncRepository<OrderRow> orderRowRepository)
        {
            this._subOrderRepository = subOrderRepository;
            this._subOrderRowRepository = subOrderRowRepository;
            this._logRecordRepository = logRecordRepository;
            this._orderRowRepository = orderRowRepository;
        }

        public async Task CreateOrder(SubOrder order)
        {
            Guard.Against.Null(order, nameof(order));
            Guard.Against.Zero(order.SubOrderRow.Count, nameof(order.SubOrderRow));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        try
                        {
                            List<OrderRow> updOrderRows = new List<OrderRow>();
                            foreach (var subOrderRow in order.SubOrderRow)
                            {
                                Guard.Against.Null(subOrderRow.OrderRowId,nameof(subOrderRow.OrderRowId));
                                OrderRowSpecification orderRowSpecification =new OrderRowSpecification(subOrderRow.OrderRowId,
                                    null,null,null,null,null,null,null,null,null);
                                List<OrderRow> orderRows =
                                    await this._orderRowRepository.ListAsync(orderRowSpecification);
                                Guard.Against.Zero(orderRows.Count,nameof(orderRows));
                                OrderRow orderRow = orderRows.First();
                                if((orderRow.PreCount - orderRow.Expend)>subOrderRow.PreCount)
                                    throw new Exception(string.Format("行数量大于前置订单行[{0}]剩余数量[{1}]",subOrderRow.OrderRowId,orderRow.Expend));
                                orderRow.Expend += subOrderRow.PreCount;
                                updOrderRows.Add(orderRow);
                            }
                            this._orderRowRepository.Update(updOrderRows);
                            SubOrder newSubOrder =  this._subOrderRepository.Add(order);
                            order.SubOrderRow.ForEach(or=>or.SubOrderId=newSubOrder.Id);
                            this._subOrderRowRepository.Add(order.SubOrderRow);
                            scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw ex;
                }
            }

        }

        public async Task ScrapOrder(SubOrder order)
        {
            Guard.Against.Null(order, nameof(order));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        try
                        {
                            
                            SubOrderSpecification subOrderSpecification = new SubOrderSpecification(order.Id,null,
                                null,null,null,null,null,null,null,null,
                                null,null,null);
                            List<SubOrder> subOrders = await this._subOrderRepository.ListAsync(subOrderSpecification);
                            Guard.Against.Zero(subOrders.Count,nameof(subOrders));
                            SubOrder subOrder = subOrders.First();
                            
                            if(subOrder.Status!=Convert.ToInt32(ORDER_STATUS.待处理))
                                throw new Exception(string.Format("订单[{0}]状态必须为待处理才能作废!",subOrder.Id));
                            subOrder.Status = Convert.ToInt32(ORDER_STATUS.关闭);
                            
                            SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(null,
                                subOrder.Id,null,null,null,null,null,
                                null,null,null);
                            List<SubOrderRow> subOrderRows = await this._subOrderRowRepository.ListAsync(subOrderRowSpecification);
                            List<OrderRow> updOrderRows = new List<OrderRow>();
                            foreach (var subOrderRow in subOrderRows)
                            {
                                if (subOrderRow.Status!=Convert.ToInt32(ORDER_STATUS.待处理))
                                    throw new Exception(string.Format("订单行[{0}]状态必须为待处理才能作废!",subOrderRow.Id));
                                subOrderRow.Status = Convert.ToInt32(ORDER_STATUS.关闭);
                                OrderRowSpecification orderRowSpecification =new OrderRowSpecification(subOrderRow.OrderRowId,
                                    null,null,null,null,null,null,null,null,null);
                                List<OrderRow> orderRows =
                                    await this._orderRowRepository.ListAsync(orderRowSpecification);
                                Guard.Against.Zero(orderRows.Count,nameof(orderRows));
                                OrderRow orderRow = orderRows.First();
                                orderRow.Expend -= subOrderRow.PreCount;
                                updOrderRows.Add(orderRow);
                            }
                            this._orderRowRepository.Update(updOrderRows);
                            this._subOrderRowRepository.Update(subOrderRows);
                            this._subOrderRepository.Update(subOrder);
                            scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw ex;
                }
            }
        }

        public async Task ScrapOrderRow(List<SubOrderRow> subOrderRows)
        {
            Guard.Against.Zero(subOrderRows.Count, nameof(subOrderRows));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        try
                        {
                            List<OrderRow> updOrderRows = new List<OrderRow>();
                            foreach (var sr in subOrderRows)
                            {
                                SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(sr.Id,
                                    null,null,null,null,null,null,
                                    null,null,null);
                                List<SubOrderRow> findSubOrderRows = await this._subOrderRowRepository.ListAsync(subOrderRowSpecification);
                                SubOrderRow subOrderRow = findSubOrderRows.First();
                                if (subOrderRow.Status!=Convert.ToInt32(ORDER_STATUS.待处理))
                                    throw new Exception(string.Format("订单行[{0}]状态必须为待处理才能作废!",subOrderRow.Id));
                                sr.Status = Convert.ToInt32(ORDER_STATUS.关闭);
                                OrderRowSpecification orderRowSpecification =new OrderRowSpecification(subOrderRow.OrderRowId,
                                    null,null,null,null,null,null,null,null,null);
                                List<OrderRow> orderRows =
                                    await this._orderRowRepository.ListAsync(orderRowSpecification);
                                Guard.Against.Zero(orderRows.Count,nameof(orderRows));
                                OrderRow orderRow = orderRows.First();
                                orderRow.Expend -= subOrderRow.PreCount;
                                updOrderRows.Add(orderRow);
                            }
                            this._orderRowRepository.Update(updOrderRows);
                            this._subOrderRowRepository.Update(subOrderRows);
                            scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw ex;
                }
            }
        }
    }
}