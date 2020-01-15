using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 库区实体类
    /// </summary>
    public class ReservoirArea : BaseEntity
    {
        public string AreaName { get; set; }
        public int OrganizationId { get; set; }
        public int OUId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string Memo { get; set; }
        public Warehouse Warehouse { get; set; }
        public Organization Organization { get; set; }
        public OU OU { get; set; }
    }
}
