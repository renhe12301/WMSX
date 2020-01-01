using System;
using ApplicationCore.Entities.OrganizationManager;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 仓库实体类
    /// </summary>
    public class Warehouse:BaseEntity
    {
        public int? SourceId { get; set; }
        public string WhName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string Memo { get; set; }
        public int OrgId { get; set; }
        public string WCSWebUrl { get; set; }
        public Organization Organization { get; set; }
        
    }
}
