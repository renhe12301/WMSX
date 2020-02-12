using System;

namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 仓库实体类
    /// </summary>
    public class Warehouse:BaseEntity
    {
        public string WhCode { get; set; }
        public string WhName { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public string Memo { get; set; }
        public int OUId { get; set; }
        public OU OU { get; set; }
        
    }
}
