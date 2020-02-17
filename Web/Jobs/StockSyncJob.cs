using System;
using System.Threading.Tasks;
using Quartz;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Specifications;

namespace Web.Jobs
{
    public class JyhStockJob: IJob
    {
        private readonly IInOutRecordService _inOutRecordService;
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRepository;


        public JyhStockJob(IInOutRecordService inOutRecordService,
                             IAsyncRepository<InOutRecord> inOutRecordRepository)
        {
            this._inOutRecordRepository = inOutRecordRepository;
            this._inOutRecordService = inOutRecordService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                InOutRecordSpecification inOutRecordSpec = new InOutRecordSpecification(null,null,null,
                    null,null,null,null,0,null,null);
                var records = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
                foreach (var record in records)
                {
                    try
                    {
                        // todo 集约化系统库存反馈
                        record.IsRead = 1;
                        await this._inOutRecordRepository.UpdateAsync(record);
                    }
                    catch (Exception ex)
                    {
                        // todo 添加异常日志记录
                    }
                }
            }
            catch (Exception ex)
            {
                // todo 添加异常日志记录
            }
        }
    }
}
