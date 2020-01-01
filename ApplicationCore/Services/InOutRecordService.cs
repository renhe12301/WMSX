using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class InOutRecordService:IInOutRecordService
    {
        private IAsyncRepository<InOutRecord> _inOutRecordRepository;

        public InOutRecordService(IAsyncRepository<InOutRecord> inOutRecordRepository)
        {
            this._inOutRecordRepository = inOutRecordRepository;
        }

        public async Task AddInOutRecord(InOutRecord inOutRecord)
        {
            Guard.Against.Null(inOutRecord, nameof(inOutRecord));
            await this._inOutRecordRepository.AddAsync(inOutRecord);
        }

        public async Task UpdateInOutRecord(InOutRecord inOutRecord)
        {
            Guard.Against.Null(inOutRecord, nameof(inOutRecord));
            await this._inOutRecordRepository.UpdateAsync(inOutRecord);
        }
    }
}
