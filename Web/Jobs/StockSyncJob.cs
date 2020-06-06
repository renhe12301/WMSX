using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Quartz;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Jobs.JYHWSDL;

namespace Web.Jobs
{
    /// <summary>
    /// 库存同步处理定时任务
    /// </summary>
    public class StockSyncJob: IJob
    {
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        public StockSyncJob()
        {
            this._logRecordRepository = EnginContext.Current.Resolve<IAsyncRepository<LogRecord>>();
            this._subOrderRepository = EnginContext.Current.Resolve<IAsyncRepository<SubOrder>>();
            this._subOrderRowRepository = EnginContext.Current.Resolve<IAsyncRepository<SubOrderRow>>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    SubOrderSpecification subOrderSpecification = new SubOrderSpecification(null, null, null, null,
                        new List<int> {Convert.ToInt32(ORDER_STATUS.完成)}, null, 0, null, null, null, null,
                        null, null, null, null, null, null, null);
                    List<SubOrder> subOrders = this._subOrderRepository.List(subOrderSpecification);

                    foreach (var subOrder in subOrders)
                    {
                        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            try
                            {

                                SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(null,
                                    subOrder.Id,
                                    null, null, null, null, null, null, null, null, null,
                                    null, null, null, null, null, null);
                                List<SubOrderRow> subOrderRows =
                                    this._subOrderRowRepository.List(subOrderRowSpecification);

                                if (subOrder.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收))
                                {
                                    WarehouseReceiptPort warehouseReceiptPort = new WarehouseReceiptPortClient();
                                    RKOrderRequest1 rkOrderRequest = new RKOrderRequest1();
                                    rkOrderRequest.RKOrderRequest = new RKOrderRequest();
                                    rkOrderRequest.RKOrderRequest.headId = subOrder.Id;
                                    rkOrderRequest.RKOrderRequest.documentNumber = subOrder.OrderNumber;
                                    rkOrderRequest.RKOrderRequest.documentType = subOrder.SourceOrderType;
                                    rkOrderRequest.RKOrderRequest.ouCode = subOrder.OU.Id.ToString();
                                    rkOrderRequest.RKOrderRequest.organizationCode = subOrder.Warehouse.Id.ToString();
                                    rkOrderRequest.RKOrderRequest.vendorId = subOrder.SupplierId.ToString();
                                    rkOrderRequest.RKOrderRequest.vendorSiteId = subOrder.SupplierSiteId.ToString();
                                    rkOrderRequest.RKOrderRequest.currency = subOrder.Currency;
                                    rkOrderRequest.RKOrderRequest.totalAmount = subOrder.TotalAmount;
                                    rkOrderRequest.RKOrderRequest.creationDate = subOrder.CreateTime.Value;
                                    rkOrderRequest.RKOrderRequest.remark = subOrder.Memo;
                                    rkOrderRequest.RKOrderRequest.requestRKRows = new RequestRKRow[subOrders.Count];

                                    for (int i = 0; i < subOrderRows.Count; i++)
                                    {
                                        RequestRKRow requestRkRow = new RequestRKRow();
                                        rkOrderRequest.RKOrderRequest.requestRKRows[i] = requestRkRow;
                                        requestRkRow.lineId = subOrderRows[i].Id;
                                        requestRkRow.headId = subOrderRows[i].SubOrderId;
                                        requestRkRow.sourceLineId = subOrderRows[i].SourceId.GetValueOrDefault();
                                        requestRkRow.materialId = subOrderRows[i].MaterialDicId.ToString();
                                        requestRkRow.processingQuantity = subOrderRows[i].PreCount;
                                        requestRkRow.price = subOrderRows[i].Price.Value;
                                        requestRkRow.amount = subOrderRows[i].Amount.Value;
                                        requestRkRow.inventoryCode = subOrderRows[i].ReservoirArea.Id.ToString();
                                    }

                                    var response = await warehouseReceiptPort.RKOrderAsync(rkOrderRequest);

                                    subOrder.IsSync = 1;
                                    this._subOrderRepository.Update(subOrder);
                                }

                                else if (subOrder.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料))
                                {
                                    StockOutOrderPort stockOutOrderPort = new StockOutOrderPortClient();
                                    CKOrderRequest1 ckOrderRequest = new CKOrderRequest1();
                                    ckOrderRequest.CKOrderRequest = new CKOrderRequest();
                                    ckOrderRequest.CKOrderRequest.headId = subOrder.Id;
                                    ckOrderRequest.CKOrderRequest.documentNumber = subOrder.OrderNumber;
                                    ckOrderRequest.CKOrderRequest.documentType = subOrder.SourceOrderType;
                                    ckOrderRequest.CKOrderRequest.ouCode = subOrder.OU.Id.ToString();
                                    ckOrderRequest.CKOrderRequest.organizationCode = subOrder.Warehouse.Id.ToString();
                                    ckOrderRequest.CKOrderRequest.creationDate = subOrder.CreateTime.Value;
                                    ckOrderRequest.CKOrderRequest.requestCKRows = new RequestCKRow[subOrders.Count];

                                    for (int i = 0; i < subOrderRows.Count; i++)
                                    {
                                        RequestCKRow requestCkRow = new RequestCKRow();
                                        ckOrderRequest.CKOrderRequest.requestCKRows[i] = requestCkRow;
                                        requestCkRow.lineId = subOrderRows[i].Id;
                                        requestCkRow.headId = subOrderRows[i].SubOrderId;
                                        requestCkRow.sourceLineId = subOrderRows[i].SourceId.GetValueOrDefault();
                                        requestCkRow.materialId = subOrderRows[i].MaterialDicId.ToString();
                                        requestCkRow.processingQuantity = subOrderRows[i].PreCount;
                                        requestCkRow.inventoryCode = subOrderRows[i].ReservoirArea.Id.ToString();
                                    }

                                    var response = await stockOutOrderPort.CKOrderAsync(ckOrderRequest);

                                    subOrder.IsSync = 1;
                                    this._subOrderRepository.Update(subOrder);
                                }
                                else if (subOrder.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库))
                                {
                                    WithdrawalPort withdrawalPort = new WithdrawalPortClient();
                                    TKOrderRequest1 tkOrderRequest = new TKOrderRequest1();
                                    tkOrderRequest.TKOrderRequest = new TKOrderRequest();
                                    tkOrderRequest.TKOrderRequest.headId = subOrder.Id;
                                    tkOrderRequest.TKOrderRequest.documentNumber = subOrder.OrderNumber;
                                    tkOrderRequest.TKOrderRequest.documentType = subOrder.SourceOrderType;
                                    tkOrderRequest.TKOrderRequest.ouCode = subOrder.OU.Id.ToString();
                                    tkOrderRequest.TKOrderRequest.organizationCode = subOrder.Warehouse.Id.ToString();
                                    tkOrderRequest.TKOrderRequest.creationDate = subOrder.CreateTime.Value;
                                    tkOrderRequest.TKOrderRequest.requestTKRows = new RequestTKRow[subOrders.Count];

                                    for (int i = 0; i < subOrderRows.Count; i++)
                                    {
                                        RequestTKRow requestTkRow = new RequestTKRow();
                                        tkOrderRequest.TKOrderRequest.requestTKRows[i] = requestTkRow;
                                        requestTkRow.lineId = subOrderRows[i].Id;
                                        requestTkRow.headId = subOrderRows[i].SubOrderId;
                                        requestTkRow.sourceLineId = subOrderRows[i].SourceId.GetValueOrDefault();
                                        requestTkRow.materialId = subOrderRows[i].MaterialDicId.ToString();
                                        requestTkRow.processingQuantity = subOrderRows[i].PreCount;
                                        requestTkRow.inventoryCode = subOrderRows[i].ReservoirArea.Id.ToString();
                                    }

                                    var response = await withdrawalPort.TKOrderAsync(tkOrderRequest);

                                    subOrder.IsSync = 1;
                                    this._subOrderRepository.Update(subOrder);
                                }
                                else if (subOrder.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退料))
                                {
                                    InboundReturnsPort inboundReturnsPort = new InboundReturnsPortClient();
                                    RTOrderRequest1 rtOrderRequest1 = new RTOrderRequest1();
                                    rtOrderRequest1.RTOrderRequest = new RTOrderRequest();
                                    rtOrderRequest1.RTOrderRequest.headId = subOrder.Id;
                                    rtOrderRequest1.RTOrderRequest.documentNumber = subOrder.OrderNumber;
                                    rtOrderRequest1.RTOrderRequest.documentType = subOrder.SourceOrderType;
                                    rtOrderRequest1.RTOrderRequest.ouCode = subOrder.OU.Id.ToString();
                                    rtOrderRequest1.RTOrderRequest.organizationCode = subOrder.Warehouse.Id.ToString();
                                    rtOrderRequest1.RTOrderRequest.creationDate = subOrder.CreateTime.Value;
                                    rtOrderRequest1.RTOrderRequest.requestRTRows = new RequestRTRow[subOrders.Count];

                                    for (int i = 0; i < subOrderRows.Count; i++)
                                    {
                                        RequestRTRow requestRtRow = new RequestRTRow();
                                        rtOrderRequest1.RTOrderRequest.requestRTRows[i] = requestRtRow;
                                        requestRtRow.lineId = subOrderRows[i].Id;
                                        requestRtRow.headId = subOrderRows[i].SubOrderId;
                                        requestRtRow.sourceLineId = subOrderRows[i].SourceId.GetValueOrDefault();
                                        requestRtRow.materialId = subOrderRows[i].MaterialDicId.ToString();
                                        requestRtRow.processingQuantity = subOrderRows[i].PreCount;
                                        requestRtRow.inventoryCode = subOrderRows[i].ReservoirArea.Id.ToString();
                                    }

                                    var response = await inboundReturnsPort.RTOrderAsync(rtOrderRequest1);

                                    subOrder.IsSync = 1;
                                    this._subOrderRepository.Update(subOrder);
                                }
                            }
                            catch (Exception ex)
                            {
                                this._logRecordRepository.Add(new LogRecord
                                {
                                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                                    LogDesc = ex.StackTrace,
                                    CreateTime = DateTime.Now
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
    
    
    
}
