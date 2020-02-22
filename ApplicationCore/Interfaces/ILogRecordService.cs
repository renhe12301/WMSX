using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Interfaces
{
    public interface ILogRecordService
    {
        Task AddLog( LogRecord logRecord);
        Task DeleteLog(List<LogRecord> logRecords);
    }
}