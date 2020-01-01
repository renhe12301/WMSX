using System;
namespace Web.ViewModels.OrderManager
{
    public class OrderRowViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int SourceId { get; set; }
        public string CreateTime { get; set; }
        public string FinishTime { get; set; }
        public int PreCount { get; set; }
        public int RealityCount { get; set; }
        public int Sorting { get; set; }
        public int Progress { get; set; }
        public int WarehouseId { get; set; }
        public int? ReservoirAreaId { get; set; }
        public string WarehouseName { get; set; }
        public string ReservoirAreaName { get; set; }
        public string Memo { get; set; }
        public string TrayCode { get; set; }
    }
}
