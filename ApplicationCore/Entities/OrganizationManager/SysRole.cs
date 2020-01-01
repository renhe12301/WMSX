using System;
using System.Collections.Generic;
namespace ApplicationCore.Entities.OrganizationManager
{
    /// <summary>
    /// 系统角色实体类
    /// </summary>
    public class SysRole : BaseEntity
    {
        public int ParentId { get; set; }
        public string RoleName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
    }
}
