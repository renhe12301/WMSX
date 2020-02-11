using System;
using System.Collections.Generic;
namespace ApplicationCore.Entities.AuthorityManager
{
    public class SysRole : BaseEntity
    {
        public string RoleName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
        public int Status { get; set; }
    }
}
