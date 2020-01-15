using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Entities.StockManager
{
    /// <summary>
    /// 库存托盘实体类
    /// </summary>
    public class WarehouseTray : BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public string Code { get; set; }
        public int TrayDicId { get; set; }
        public int? LocationId { get; set; }
        public int MaterialCount { get; set; }
        public int OutCount { get; set; }
        public int? OrganizationId { get; set; }
        public int? OUId { get; set; }
        public int? WarehouseId { get; set; }
        public int? ReservoirAreaId { get; set; }
        public Organization Organization { get; set; }
        public OU OU { get; set; }
        public int? TrayStep { get; set; }
        public DateTime CreateTime { get; set; }
        public int? Carrier { get; set; }
        public string Memo { get; set; }
        public TrayDic TrayDic { get; set; }
        public Location Location { get; set; }
        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
        public Order Order { get; set; }
        public OrderRow OrderRow { get; set; }
        public List<WarehouseMaterial> WarehouseMaterial { get; set; }
    }
}
