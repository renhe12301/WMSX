using System;
using System.Collections.Generic;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 组织架构实体类
    /// </summary>
    public class Organization : BaseEntity
    {
        public string OrgName { get; set; }
        public string OrgCode { get; set; }
        public int? OUId { get; set; }
        public OU OU { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Memo { get; set; }

    }
}
