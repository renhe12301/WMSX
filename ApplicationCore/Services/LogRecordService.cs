using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class LogRecordService:ILogRecordService
    {
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        
        public LogRecordService(IAsyncRepository<LogRecord> logRecordRepository)
        {
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddLog(LogRecord logRecord)
        {
            try
            {
                Guard.Against.Null(logRecord, nameof(logRecord));
                await this._logRecordRepository.AddAsync(logRecord);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task DeleteLog(List<LogRecord> logRecords)
        {
            Guard.Against.Null(logRecords,nameof(logRecords));
            Guard.Against.NullOrEmpty(logRecords,nameof(logRecords));
            await this._logRecordRepository.DeleteAsync(logRecords);
        }
    }
}