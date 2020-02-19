using System;

namespace ApplicationCore.Entities.BasicInformation
{
    public class LogRecord:BaseEntity
    {
        public int LogType { get; set; }
        public string LogDesc { get; set; }
        public DateTime CreateTime { get; set; }
    }
}