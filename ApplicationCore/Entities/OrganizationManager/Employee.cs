using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities.OrganizationManager
{
    public class Employee:BaseEntity
    {
        public string LoginName { get; set; }
        public string LoinPwd { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public int ParentId { get; set; }
        public int Type { get; set; }
        public string Memo { get; set; }
        
    }
}
