using System;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Specifications
{
    public class LogRecordPaginatedSpecification:BaseSpecification<LogRecord>
    {
        public LogRecordPaginatedSpecification(int skip,int take,int? logType,string logDesc,string sCreateTime,string eCreateTime):
            base(b=>(!logType.HasValue||b.LogType==logType)&&
                                      (logDesc==null||b.LogDesc.Contains(logDesc))&&
                                      (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                                      (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime)))
        {
            ApplyPaging(skip,take);
        }
    }
}