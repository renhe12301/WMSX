using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities.BasicInformation
{
    public class Employee:BaseEntity
    {
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string Sex { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Memo { get; set; }
        public string Img { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

    }
}
