using System;
using System.Threading.Tasks;
using Quartz;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.FlowRecord;
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


        public StockSyncJob(IInOutRecordService inOutRecordService,
                             IAsyncRepository<InOutRecord> inOutRecordRepository)
        {
            this._inOutRecordRepository = inOutRecordRepository;
            this._inOutRecordService = inOutRecordService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                // todo 添加异常日志记录
            }
        }
    }
}
