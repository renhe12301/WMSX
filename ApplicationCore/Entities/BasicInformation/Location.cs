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

        public int? Floor { get; set; }

        public int? Item { get; set; }

        public int? Col { get; set; }

        public DateTime? CreateTime { get; set; }
        public int Status { get; set; }
        public int InStock { get; set; }
        public int IsTask { get; set; }
        public string Memo { get; set; }
        public int? WarehouseId { get; set; }
        public int? ReservoirAreaId { get; set; }
        public int? OUId { get; set; }

        public int? PhyWarehouseId { get; set; }

        public int? Type { get; set; }
        public OU OU { get; set; }
        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }

        public PhyWarehouse PhyWarehouse { get; set; }

    }
}
