using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    [DisallowConcurrentExecution]
    public class StockSyncJob : IJob
    {
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;

        public StockSyncJob(IAsyncRepository<LogRecord> logRecordRepository,
            IAsyncRepository<SubOrder> subOrderRepository,
            IAsyncRepository<SubOrderRow> subOrderRowRepository)
        {
            this._logRecordRepository = logRecordRepository;
            this._subOrderRepository = subOrderRepository;
            this._subOrderRowRepository = subOrderRowRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                
                SubOrderSpecification subOrderSpecification = new SubOrderSpecification(null, null, null, null,null,
                    new List<int> {Convert.ToInt32(ORDER_STATUS.完成)}, null, 0,null, null, null, null, null,
                    null, null, null, null, null, null, null);
                List<SubOrder> subOrders = this._subOrderRepository.List(subOrderSpecification);
                
                foreach (var subOrder in subOrders)
                {
                    try
                    {
                        SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(null,
                            subOrder.Id,null,
                            null, null, null, null, null, null, null, null, null,null,null,
                            null, null, null, null, null, null);
                        List<SubOrderRow> subOrderRows =
                            await this._subOrderRowRepository.ListAsync(subOrderRowSpecification);

                        if (subOrder.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收))
                        {
                            WarehouseReceiptPort warehouseReceiptPort = new WarehouseReceiptPortClient();
                            RKOrderRequest1 rkOrderRequest = new RKOrderRequest1();
                            rkOrderRequest.RKOrderRequest = new RKOrderRequest();
                            rkOrderRequest.RKOrderRequest.headId = subOrder.Id;
                            rkOrderRequest.RKOrderRequest.documentNumber = subOrder.OrderNumber;
                            rkOrderRequest.RKOrderRequest.documentType = "INVENTORY";
                            rkOrderRequest.RKOrderRequest.ouCode = subOrder.OU.Id.ToString();
                            rkOrderRequest.RKOrderRequest.organizationCode = subOrder.Warehouse.Id.ToString();
                            rkOrderRequest.RKOrderRequest.vendorId = subOrder.SupplierId.ToString();
                            rkOrderRequest.RKOrderRequest.vendorSiteId = subOrder.SupplierSiteId.ToString();
                            rkOrderRequest.RKOrderRequest.currency = subOrder.Currency;
                            rkOrderRequest.RKOrderRequest.totalAmount = subOrder.TotalAmount;
                            rkOrderRequest.RKOrderRequest.creationDate = subOrder.CreateTime.Value;
                            rkOrderRequest.RKOrderRequest.remark = subOrder.Memo;
                            rkOrderRequest.RKOrderRequest.businessType = subOrder.BusinessTypeCode;
                            rkOrderRequest.RKOrderRequest.managerId = subOrder.EmployeeId.ToString();
                            rkOrderRequest.RKOrderRequest.exitEntryDate = subOrder.CreateTime.Value;
                            rkOrderRequest.RKOrderRequest.requestRKRows = new RequestRKRow[subOrderRows.Count];

                            for (int i = 0; i < subOrderRows.Count; i++)
                            {
                                RequestRKRow requestRkRow = new RequestRKRow();
                                rkOrderRequest.RKOrderRequest.requestRKRows[i] = requestRkRow;
                                requestRkRow.lineId = subOrderRows[i].Id;
                                requestRkRow.lineNumber = Convert.ToInt32(subOrderRows[i].RowNumber);
                                requestRkRow.headId = subOrderRows[i].SubOrderId;
                                requestRkRow.sourceLineId = subOrderRows[i].SourceId.GetValueOrDefault();
                                requestRkRow.materialId = subOrderRows[i].MaterialDicId.ToString();
                                requestRkRow.processingQuantity = subOrderRows[i].PreCount;
                                requestRkRow.price = subOrderRows[i].Price.Value;
                                requestRkRow.amount = subOrderRows[i].Amount.Value;
                                requestRkRow.taskId = subOrderRows[i].EBSTaskId.ToString();
                                requestRkRow.inventoryCode = subOrderRows[i].ReservoirArea.AreaCode;
                                requestRkRow.expenditureType = subOrderRows[i].ExpenditureType;
                                requestRkRow.itemId = subOrderRows[i].EBSProjectId.ToString();
                            }

                            var response = await warehouseReceiptPort.RKOrderAsync(rkOrderRequest);
                            if (response.RKOrderResponse.code == "200")
                            {
                                subOrder.IsSync = 1;
                                await this._subOrderRepository.UpdateAsync(subOrder);
                            }
                            else 
                            {
                                throw new Exception(response.RKOrderResponse.data);
                            }
                        }
                        else if (subOrder.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料))
                        {
                            StockOutOrderPort stockOutOrderPort = new StockOutOrderPortClient();
                            CKOrderRequest1 ckOrderRequest = new CKOrderRequest1();
                            ckOrderRequest.CKOrderRequest = new CKOrderRequest();
                            ckOrderRequest.CKOrderRequest.headId = subOrder.Id;
                            ckOrderRequest.CKOrderRequest.documentNumber = subOrder.OrderNumber;
                            ckOrderRequest.CKOrderRequest.documentType = "DELIVERY";
                            ckOrderRequest.CKOrderRequest.businessType = subOrder.BusinessTypeCode;
                            ckOrderRequest.CKOrderRequest.ouCode = subOrder.OU.Id.ToString();
                            ckOrderRequest.CKOrderRequest.organizationCode = subOrder.Warehouse.Id.ToString();
                            ckOrderRequest.CKOrderRequest.creationDate = subOrder.CreateTime.Value;
                            ckOrderRequest.CKOrderRequest.departmentId = subOrder.OrganizationId.ToString();
                            
                            if (subOrder.EBSProjectId.HasValue)
                                ckOrderRequest.CKOrderRequest.itemId = subOrder.EBSProjectId.ToString();
                            if(subOrder.EmployeeId.HasValue)
                                ckOrderRequest.CKOrderRequest.managerId = subOrder.EmployeeId.ToString();
                            
                            ckOrderRequest.CKOrderRequest.requestCKRows = new RequestCKRow[subOrderRows.Count];

                            for (int i = 0; i < subOrderRows.Count; i++)
                            {
                                RequestCKRow requestCkRow = new RequestCKRow();
                                ckOrderRequest.CKOrderRequest.requestCKRows[i] = requestCkRow;
                                requestCkRow.lineId = subOrderRows[i].Id;
                                requestCkRow.lineNumber = Convert.ToInt32(subOrderRows[i].RowNumber);
                                requestCkRow.headId = subOrderRows[i].SubOrderId;
                                requestCkRow.sourceLineId = subOrderRows[i].SourceId.GetValueOrDefault();
                                requestCkRow.materialId = subOrderRows[i].MaterialDicId.ToString();
                                requestCkRow.processingQuantity = subOrderRows[i].PreCount;
                                requestCkRow.expenditureType = subOrderRows[i].ExpenditureType;
                                requestCkRow.inventoryCode = subOrderRows[i].ReservoirArea.AreaCode;
                                
                                if(subOrderRows[i].EBSTaskId.HasValue)
                                   requestCkRow.taskId = subOrderRows[i].EBSTaskId.ToString();
                            }

                            var response = await stockOutOrderPort.CKOrderAsync(ckOrderRequest);
                            if (response.CKOrderResponse.code == "200")
                            {
                                subOrder.IsSync = 1;
                                await this._subOrderRepository.UpdateAsync(subOrder);
                            }
                            else
                            {
                                throw new Exception(response.CKOrderResponse.data);
                            }

                        }
                        else if (subOrder.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退料))
                        {
                            WithdrawalPort withdrawalPort = new WithdrawalPortClient();
                            TKOrderRequest1 tkOrderRequest = new TKOrderRequest1();
                            tkOrderRequest.TKOrderRequest = new TKOrderRequest();
                            tkOrderRequest.TKOrderRequest.headId = subOrder.Id;
                            tkOrderRequest.TKOrderRequest.documentNumber = subOrder.OrderNumber;
                            tkOrderRequest.TKOrderRequest.documentType = "RETURN";
                            tkOrderRequest.TKOrderRequest.ouCode = subOrder.OU.Id.ToString();
                            tkOrderRequest.TKOrderRequest.businessType = subOrder.BusinessTypeCode;
                            tkOrderRequest.TKOrderRequest.organizationCode = subOrder.Warehouse.Id.ToString();
                            tkOrderRequest.TKOrderRequest.creationDate = subOrder.CreateTime.Value;
                            tkOrderRequest.TKOrderRequest.managerId = subOrder.EmployeeId.ToString();
                            tkOrderRequest.TKOrderRequest.departmentId = subOrder.OrganizationId.ToString();
                            tkOrderRequest.TKOrderRequest.exitEntryDate = subOrder.CreateTime.Value;
                            tkOrderRequest.TKOrderRequest.totalAmount = subOrder.TotalAmount;
                            tkOrderRequest.TKOrderRequest.itemId = subOrder.EBSProjectId.ToString();
                            tkOrderRequest.TKOrderRequest.requestTKRows = new RequestTKRow[subOrderRows.Count];

                            for (int i = 0; i < subOrderRows.Count; i++)
                            {
                                RequestTKRow requestTkRow = new RequestTKRow();
                                tkOrderRequest.TKOrderRequest.requestTKRows[i] = requestTkRow;
                                requestTkRow.lineId = subOrderRows[i].Id;
                                requestTkRow.headId = subOrderRows[i].SubOrderId;
                                requestTkRow.sourceLineId = subOrderRows[i].SourceId.GetValueOrDefault();
                                requestTkRow.materialId = subOrderRows[i].MaterialDicId.ToString();
                                requestTkRow.processingQuantity = subOrderRows[i].PreCount;
                                requestTkRow.expenditureType = subOrderRows[i].ExpenditureType;
                                requestTkRow.lineNumber = Convert.ToInt32(subOrderRows[i].RowNumber);
                                requestTkRow.taskId = subOrderRows[i].EBSTaskId.ToString();
                                requestTkRow.inventoryCode = subOrderRows[i].ReservoirArea.AreaCode.ToString();
                            }

                            var response = await withdrawalPort.TKOrderAsync(tkOrderRequest);
                            if (response.TKOrderResponse.code == "200")
                            {
                                subOrder.IsSync = 1;
                                await this._subOrderRepository.UpdateAsync(subOrder);
                            }
                            else
                            {
                                throw new Exception(response.TKOrderResponse.data);
                            }
                        }
                        else if (subOrder.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退库))
                        {
                            InboundReturnsPort inboundReturnsPort = new InboundReturnsPortClient();
                            RTOrderRequest1 rtOrderRequest1 = new RTOrderRequest1();
                            rtOrderRequest1.RTOrderRequest = new RTOrderRequest();
                            rtOrderRequest1.RTOrderRequest.headId = subOrder.Id;
                            rtOrderRequest1.RTOrderRequest.documentNumber = subOrder.OrderNumber;
                            rtOrderRequest1.RTOrderRequest.documentType = "INVREFUND";
                            rtOrderRequest1.RTOrderRequest.ouCode = subOrder.OU.Id.ToString();
                            rtOrderRequest1.RTOrderRequest.businessType = subOrder.BusinessTypeCode;
                            rtOrderRequest1.RTOrderRequest.organizationCode = subOrder.Warehouse.Id.ToString();
                            rtOrderRequest1.RTOrderRequest.creationDate = subOrder.CreateTime.Value;
                            rtOrderRequest1.RTOrderRequest.managerId = subOrder.EmployeeId.ToString();
                            rtOrderRequest1.RTOrderRequest.vendorId = subOrder.SupplierId.ToString();
                            rtOrderRequest1.RTOrderRequest.vendorSiteId = subOrder.SupplierSiteId.ToString();
                            rtOrderRequest1.RTOrderRequest.exitEntryDate = subOrder.CreateTime.Value;
                            rtOrderRequest1.RTOrderRequest.totalAmount = subOrder.TotalAmount;
                            
                            
                            rtOrderRequest1.RTOrderRequest.requestRTRows = new RequestRTRow[subOrderRows.Count];


                            for (int i = 0; i < subOrderRows.Count; i++)
                            {
                                RequestRTRow requestRtRow = new RequestRTRow();
                                rtOrderRequest1.RTOrderRequest.requestRTRows[i] = requestRtRow;
                                requestRtRow.lineId = subOrderRows[i].Id;
                                requestRtRow.headId = subOrderRows[i].SubOrderId;
                                requestRtRow.sourceLineId = subOrderRows[i].SourceId.GetValueOrDefault();
                                requestRtRow.materialId = subOrderRows[i].MaterialDicId.ToString();
                                requestRtRow.processingQuantity = subOrderRows[i].PreCount;
                                requestRtRow.expenditureType = subOrderRows[i].ExpenditureType;
                                requestRtRow.lineNumber = Convert.ToInt32(subOrderRows[i].RowNumber);
                                requestRtRow.itemId = subOrderRows[i].EBSProjectId.ToString();
                                requestRtRow.taskId = subOrderRows[i].EBSTaskId.ToString();
                                requestRtRow.amount = subOrderRows[i].Amount.GetValueOrDefault();
                                requestRtRow.price = subOrderRows[i].Price.GetValueOrDefault();
                                requestRtRow.inventoryCode = subOrderRows[i].ReservoirArea.AreaCode.ToString();
                            }

                            var response = await inboundReturnsPort.RTOrderAsync(rtOrderRequest1);
                            if (response.RTOrderResponse.code == "200")
                            {
                                subOrder.IsSync = 1;
                                await this._subOrderRepository.UpdateAsync(subOrder);
                            }
                            else
                            {
                                throw new Exception(response.RTOrderResponse.data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await this._logRecordRepository.AddAsync(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                            LogDesc = " [订单完成上报集约化系统Job]:" + ex.Message,
                            CreateTime = DateTime.Now
                        });
                    }
                }

            }
            catch (Exception ex)
            {

            }

            Thread.Sleep(100);
        }

    }
}
