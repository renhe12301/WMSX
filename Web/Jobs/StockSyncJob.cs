using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;

namespace Web.Jobs
{
    /// <summary>
    /// 库存同步处理定时任务
    /// </summary>
    public class StockSyncJob: IJob
    {
        private readonly ILogRecordService _logRecordService;


        public StockSyncJob(ILogRecordService logRecordService)
        {
            this._logRecordService = logRecordService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
               //  InOutRecordSpecification inOutRecordSpec = new InOutRecordSpecification(null,null,
               //      null,null,null,null,null,null,null,
               //      null,new List<int>{Convert.ToInt32(ORDER_STATUS.完成)},0,null,null,null );
               //  List<InOutRecord> inOutRecords = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
               //  List<InOutRecord> updRecords = new List<InOutRecord>();
               //  List<OrderRowBatch> updOrderRowBatchs = new List<OrderRowBatch>();
               //  inOutRecords.ForEach(async (inOutRecord) =>
               //  {
               //      int type = inOutRecord.Type;
               //      StringBuilder sb = new StringBuilder();
               //      sb.Append("报文内容如下:\n");
               //      sb.Append(string.Format("订单Id[{0}]:\n",inOutRecord.OrderId));
               //      sb.Append(string.Format("订单行Id[{0}]:\n",inOutRecord.OrderRowId));
               //      sb.Append(string.Format("物料Id[{0}]:\n",inOutRecord.MaterialDicId));
               //      sb.Append(string.Format("物料名称[{0}]:\n",inOutRecord.MaterialDic.MaterialName));
               //      if (type == Convert.ToInt32(INOUTRECORD_FLAG.入库))
               //      {
               //          try
               //          {
               //              sb.Append(string.Format("托盘编码[{0}]:\n",inOutRecord.TrayCode));
               //              if (inOutRecord.BadCount.HasValue)
               //              {
               //                  // todo 调用集约化物资管理系统入库不合格品接口反馈
               //                  
               //                  
               //                  sb.Append(string.Format("不合格数量[{0}]:\n",inOutRecord.BadCount));
               //                  sb.Insert(0,"WMS调用集约化物资管理系统入库不合格结果反馈接口\n");
               //                  await this._logRecordService.AddLog(new LogRecord
               //                  {
               //                      LogType = Convert.ToInt32(LOG_TYPE.定时任务日志),
               //                      LogDesc = sb.ToString(),
               //                      CreateTime = DateTime.Now
               //                  });
               //              }
               //              
               //              if (inOutRecord.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收))
               //              {
               //                  // todo 调用集约化物资管理系统入库接口反馈
               //                  
               //                  
               //                  sb.Insert(0,"WMS调用集约化物资管理系统入库接收结果反馈接口\n");
               //                  await this._logRecordService.AddLog(new LogRecord
               //                  {
               //                      LogType = Convert.ToInt32(LOG_TYPE.定时任务日志),
               //                      LogDesc = sb.ToString(),
               //                      CreateTime = DateTime.Now
               //                  });
               //              }
               //              else if (inOutRecord.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退料))
               //              {
               //                  // todo 调用集约化物资管理系统入库接口反馈
               //                  
               //                  
               //                  
               //                  sb.Insert(0,"WMS调用集约化物资管理系统出库退料结果反馈接口\n");
               //                  await this._logRecordService.AddLog(new LogRecord
               //                  {
               //                      LogType = Convert.ToInt32(LOG_TYPE.定时任务日志),
               //                      LogDesc = sb.ToString(),
               //                      CreateTime = DateTime.Now
               //                  });
               //              }
               //              inOutRecord.IsRead = 1;
               //              updRecords.Add(inOutRecord);
               //          }
               //          catch (Exception ex)
               //          {
               //              await this._logRecordService.AddLog(new LogRecord
               //              {
               //                  LogType = Convert.ToInt32(LOG_TYPE.异常日志),
               //                  LogDesc = ex.StackTrace,
               //                  CreateTime = DateTime.Now
               //              });
               //          }
               //      }
               //      else if(type == Convert.ToInt32(INOUTRECORD_FLAG.出库))
               //      {
               //          try
               //          {
               //              if (!updRecords.Contains(inOutRecord))
               //              {
               //                  sb.Append(string.Format("出库批次数量[{0}]:\n",inOutRecord.OrderRowBatch.BatchCount));
               //                  
               //                  InOutRecordSpecification childSpec = new InOutRecordSpecification(null,null,null,
               //                      null,null,null,null,null,inOutRecord.OrderRowBatchId,
               //                      null,null,null,null,null,null );
               //                  List<InOutRecord> childInOutRecords =
               //                      await this._inOutRecordRepository.ListAsync(childSpec);
               //                  List<InOutRecord> finishInOutRecords =
               //                      childInOutRecords.Where(r => r.Status == Convert.ToInt32(ORDER_STATUS.完成)).ToList();
               //                  if (finishInOutRecords.Count() == childInOutRecords.Count())
               //                  {
               //                      if (inOutRecord.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料))
               //                      {
               //                          // todo 调用集约化物资管理系统入库接口反馈
               //                          
               //                          
               //                          sb.Insert(0,"WMS调用集约化物资管理系统出库领料结果反馈接口\n");
               //                          await this._logRecordService.AddLog(new LogRecord
               //                          {
               //                              LogType = Convert.ToInt32(LOG_TYPE.定时任务日志),
               //                              LogDesc = sb.ToString(),
               //                              CreateTime = DateTime.Now
               //                          });
               //                      }
               //                      else if (inOutRecord.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库))
               //                      {
               //                          // todo 调用集约化物资管理系统入库接口反馈
               //                  
               //                  
               //                  
               //                          sb.Insert(0,"WMS调用集约化物资管理系统入库退库结果反馈接口\n");
               //                          await this._logRecordService.AddLog(new LogRecord
               //                          {
               //                              LogType = Convert.ToInt32(LOG_TYPE.定时任务日志),
               //                              LogDesc = sb.ToString(),
               //                              CreateTime = DateTime.Now
               //                          });
               //                      }
               //                      finishInOutRecords.ForEach(r=>r.IsRead=1);
               //                      updRecords.AddRange(finishInOutRecords);
               //                      updOrderRowBatchs.Add(inOutRecord.OrderRowBatch);
               //                  }
               //              }
               //          }
               //          catch (Exception ex)
               //          {
               //              await this._logRecordService.AddLog(new LogRecord
               //              {
               //                  LogType = Convert.ToInt32(LOG_TYPE.异常日志),
               //                  LogDesc = ex.StackTrace,
               //                  CreateTime = DateTime.Now
               //              });
               //          }
               //      }
               //  });
               //
               // await this._inOutRecordRepository.UpdateAsync(updRecords);
               // await this._orderRowBatchRepository.UpdateAsync(updOrderRowBatchs);

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
