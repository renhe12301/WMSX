using System;
using ApplicationCore.Entities.OrganizationManager;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 仓库实体类
    /// </summary>
    public class Warehouse:BaseEntity
    {
        public string WhName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string Memo { get; set; }
        
        public int OrganizationId { get; set; }
        public int OUId { get; set; }
        public Organization Organization { get; set; }
        public OU OU { get; set; }
        
    }
}
