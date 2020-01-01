using System;
namespace Web.ViewModels.FlowRecord
{
    public class InOutRecordViewModel
    {
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public int WarehouseId { get; set; }
        public int ReservoirAreaId { get; set; }
        public int TrayDicId { get; set; }
        public int MaterialDicId { get; set; }
        public int InOutCount { get; set; }
        public int IsRead { get; set; }
        public string CreateTime { get; set; }
        public int Type { get; set; }
        public int InOutFlag { get; set; }
        public string WarehouseName { get; set; }
        public string ReservoirAreaName { get; set; }
        public string MaterialDicName { get; set; }
        public string TrayDicName { get; set; }
    }
}
