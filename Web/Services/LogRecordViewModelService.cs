using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using Web.ViewModels.FlowRecord;
using LogRecord = ApplicationCore.Entities.FlowRecord.LogRecord;

namespace Web.Services
{
    public class LogRecordViewModelService:ILogRecordViewModelService
    {
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;

        public LogRecordViewModelService(IAsyncRepository<LogRecord> logRecordRepository)
        {
            this._logRecordRepository = logRecordRepository;
        }

        public async Task<ResponseResultViewModel> GetLogRecords(int? pageIndex, int? itemsPage, string logTypes, string logDesc, 
                                                           string founder,string sCreateTime, string eCreateTIme)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                
                List<int> types = null;
                if (!string.IsNullOrEmpty(logTypes))
                {
                    types = logTypes.Split(new char[]
                    {
                        ','
                    }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }

                BaseSpecification<LogRecord> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new LogRecordPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        types, logDesc, founder, sCreateTime, eCreateTIme);
                }
                else
                {
                    baseSpecification = new LogRecordSpecification(types, logDesc, founder, sCreateTime, eCreateTIme);
                }

                var logRecords = await this._logRecordRepository.ListAsync(baseSpecification);
                List<LogRecordViewModel> logRecordViewModels = new List<LogRecordViewModel>();

                logRecords.ForEach(e =>
                {
                    LogRecordViewModel logRecordViewModel = new LogRecordViewModel
                    {
                        Id = e.Id,
                        LogType = Enum.GetName(typeof(LOG_TYPE), e.LogType),
                        LogDesc = e.LogDesc,
                        Founder = e.Founder,
                        CreateTime = e.CreateTime.ToString()
                    };
                    logRecordViewModels.Add(logRecordViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._logRecordRepository.CountAsync(new LogRecordSpecification(types, logDesc,
                        founder, sCreateTime, eCreateTIme));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = logRecordViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = logRecordViewModels;
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
    }
}