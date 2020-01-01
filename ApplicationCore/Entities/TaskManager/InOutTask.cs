using System;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.StockManager;

namespace ApplicationCore.Entities.TaskManager
{
    public class InOutTask : BaseEntity
    {
        public int OrderId{ get; set; }
        public int OrderRowId { get; set; }
        public int WarehouseTrayId { get; set; }
        public string SrcId { get; set; }
        public string TargetId { get; set; }
        public int Status { get; set; }
        public int Step { get; set; }
        public int Progress { get; set; }
        public int Feedback { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime FinishTime { get; set; }
        public int IsRead { get; set; }
        public string Memo { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; }
        public Order Order { get; set; }
        public OrderRow OrderRow { get; set; }
        public WarehouseTray WarehouseTray { get; set; }
    }
}
