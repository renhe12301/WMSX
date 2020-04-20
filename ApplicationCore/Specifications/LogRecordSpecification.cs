using System;
using System.Collections.Generic;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Specifications
{
    public class LogRecordSpecification:BaseSpecification<LogRecord>
    {
        public LogRecordSpecification(List<int> logTypes,string logDesc,string founder,string sCreateTime,string eCreateTime):
            base(b=>(logTypes==null||logTypes.Contains(b.LogType))&&
                                      (logDesc==null||b.LogDesc.Contains(logDesc))&&
                                      (founder==null||b.Founder.Contains(founder))&&
                                      (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                                      (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime)))
        {
            ApplyOrderByDescending(b => b.CreateTime);
        }
    }
}