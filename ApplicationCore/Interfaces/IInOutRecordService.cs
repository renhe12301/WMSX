using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Interfaces
{
    public interface IInOutRecordService
    {
        public Task AddInOutRecord(InOutRecord inOutRecord);
        public Task UpdateInOutRecord(InOutRecord inOutRecord);
    }
}
