using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quartz;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;

namespace Web.Jobs
{
    /// <summary>
    /// 库存同步处理定时任务
    /// </summary>
    public class StockSyncJob: IJob
    {
        private readonly IInOutRecordService _inOutRecordService;
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRepository;
        private readonly ILogRecordService _logRecordService;


        public StockSyncJob(IInOutRecordService inOutRecordService,
                             IAsyncRepository<InOutRecord> inOutRecordRepository,
                             ILogRecordService logRecordService)
        {
            this._inOutRecordRepository = inOutRecordRepository;
            this._inOutRecordService = inOutRecordService;
            this._logRecordService = logRecordService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                InOutRecordSpecification inOutRecordSpec = new InOutRecordSpecification(null,null,null,null,null,
                    null,null,null,null,new List<int>{Convert.ToInt32(ORDER_STATUS.完成)},0,null,null,null );
                List<InOutRecord> inOutRecords = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
                List<InOutRecord> updRecords = new List<InOutRecord>();
                inOutRecords.ForEach(async (inOutRecord) =>
                {
                    int type = inOutRecord.Type;
                    if (type == Convert.ToInt32(INOUTRECORD_FLAG.入库))
                    {
                        try
                        {
                            if (inOutRecord.BadCount.HasValue)
                            {
                                // todo 调用集约化物资管理系统入库不合格品接口反馈
                            }
                            
                            if (inOutRecord.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收))
                            {
                                // todo 调用集约化物资管理系统入库接口反馈
                            }
                            else if (inOutRecord.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.出库退料))
                            {
                                // todo 调用集约化物资管理系统入库接口反馈
                            }
                            inOutRecord.IsRead = 1;
                            updRecords.Add(inOutRecord);
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
                    else if(type == Convert.ToInt32(INOUTRECORD_FLAG.出库))
                    {
                        try
                        {
                            
                            inOutRecord.IsRead = 1;
                            updRecords.Add(inOutRecord);
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
                });

               await this._inOutRecordRepository.UpdateAsync(updRecords);

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
