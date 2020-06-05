using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Entities.StockManager
{
    /// <summary>
    /// 库存托盘实体类
    /// </summary>
    public class WarehouseTray : BaseEntity
    {
        public int? SubOrderId { get; set; }
        public int? SubOrderRowId { get; set; }
        public string TrayCode { get; set; }
        public int? LocationId { get; set; }
        public double MaterialCount { get; set; }
        public double? OutCount { get; set; }
        public int? OUId { get; set; }
        public int? WarehouseId { get; set; }
        public int? ReservoirAreaId { get; set; }

        public OU OU { get; set; }
        public int? TrayStep { get; set; }
        public DateTime CreateTime { get; set; }
        public int? Carrier { get; set; }

        public double? Price { get; set; }
        public double? Amount { get; set; }

        public string Memo { get; set; }

        public int? CargoHeight { get; set; }

        public string CargoWeight { get; set; }

        public Location Location { get; set; }
        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
        public SubOrder SubOrder { get; set; }
        public SubOrderRow SubOrderRow { get; set; }
        
        /// <summary>
        /// 物理仓库编号
        /// </summary>
        
        public int? PhyWarehouseId { get; set; }
        
        public PhyWarehouse PhyWarehouse { get; set; }

        [NotMapped]
        public List<WarehouseMaterial> WarehouseMaterial { get; set; }
    }
}
