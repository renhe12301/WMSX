using System;
using System.Collections.Generic;
namespace ApplicationCore.Entities.OrganizationManager
{
    /// <summary>
    /// 组织架构实体类
    /// </summary>
    public class Organization : BaseEntity
    {
        public int? SourceId { get; set; }
        public string Code { get; set; }
        public string OrgName { get; set; }
        public int ParentId { get; set; }
        public string Address { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
