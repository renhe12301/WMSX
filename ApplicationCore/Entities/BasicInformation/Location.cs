using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 货位实体类
    /// </summary>
    public class Location:BaseEntity
    {
        public string SysCode { get; set; }
        public string UserCode { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int InStock { get; set; }
        public string Memo { get; set; }
        public int Type { get; set; }
        public int WarehouseId { get; set; }
        public int? ReservoirAreaId { get; set; }
        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
    }
}
