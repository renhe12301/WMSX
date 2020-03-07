using System;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Entities.TaskManager
{
    public class InOutTask : BaseEntity
    {
        public int? OrderId{ get; set; }
        public int? OrderRowId { get; set; }
        public string TrayCode { get; set; }
        public string SrcId { get; set; }
        public string TargetId { get; set; }
        public int Status { get; set; }
        public int Step { get; set; }
        public int Progress { get; set; }
        public int Type { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int IsRead { get; set; }
        public string Memo { get; set; }
        public int? VehicleId { get; set; }
        public int? OUId { get; set; }
        public int? WarehouseId { get; set; }
        public int? ReservoirAreaId { get; set; }
        
        public int? PhyWarehouseId { get; set; }
        
        public PhyWarehouse PhyWarehouse { get; set; }

        public OU OU { get; set; }

        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
        public Order Order { get; set; }
        public OrderRow OrderRow { get; set; }
    }
}
