using System;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;
namespace ApplicationCore.Entities.StockManager
{
  
    public class WarehouseMaterial:BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public int MaterialDicId { get; set; }
        public int WarehouseTrayId { get; set; }
        public int MaterialCount { get; set; }
        public string BatchNo { get; set; }
        public int LocationId { get; set; }
        public int WarehouseId { get; set; }
        public int ReservoirAreaId { get; set; }
        public DateTime CreateTime { get; set; }
        public int? Carrier { get; set; }
        public string Memo { get; set; }
        public WarehouseTray WarehouseTray { get; set; }
        public MaterialDic MaterialDic { get; set; }
        public Location Location { get; set; }
        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
        public Order Order { get; set; }
        public OrderRow OrderRow { get; set; }
    }
}
