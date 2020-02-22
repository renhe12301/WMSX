using System;

namespace ApplicationCore.Entities.FlowRecord
{
    public class LogRecord:BaseEntity
    {
        public int LogType { get; set; }
        public string LogDesc { get; set; }
        public DateTime CreateTime { get; set; }
        public string Founder { get; set; }
    }
}