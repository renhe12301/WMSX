using System;

namespace ApplicationCore.Entities.BasicInformation
{
    public class EBSTask:BaseEntity
    {
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public string Summary { get; set; }
        public string TaskLevel { get; set; }
        public int EBSProjectId { get; set; }
        public EBSProject EBSProject { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}