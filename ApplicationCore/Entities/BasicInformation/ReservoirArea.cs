using System;
using System.Collections.Generic;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 库区实体类
    /// </summary>
    public class ReservoirArea : BaseEntity
    {
        public int? SourceId { get; set; }
        public string AreaName { get; set; }
        public int WarehouseId { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string Memo { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
