using System;

namespace ApplicationCore.Entities.BasicInformation
{
    public class EBSProject:BaseEntity
    {
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectFullName { get; set; }
        public int OUId { get; set; }
        public OU OU { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string TypeCode { get; set; }
        public string StatusCode { get; set; }
        public string ProjectCategory { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}