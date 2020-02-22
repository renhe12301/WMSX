using System;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Specifications
{
    public class LogRecordPaginatedSpecification:BaseSpecification<LogRecord>
    {
        public LogRecordPaginatedSpecification(int skip,int take,int? logType,string logDesc,string founder,
                                               string sCreateTime,string eCreateTime):
            base(b=>(!logType.HasValue||b.LogType==logType)&&
                                      (logDesc==null||b.LogDesc.Contains(logDesc))&&
                                      (founder==null||b.Founder.Contains(founder))&&
                                      (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                                      (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime)))
        {
            ApplyPaging(skip,take);
        }
    }
}