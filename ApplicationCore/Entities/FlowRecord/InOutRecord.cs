using System;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Entities.FlowRecord
{
    public class InOutRecord:BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public int WarehouseId { get; set; }
        public int ReservoirAreaId { get; set; }
        public int? OUId { get; set; }
        public string TrayCode { get; set; }
        public int MaterialDicId { get; set; }
        public int InOutCount { get; set; }
        public int IsRead { get; set; }
        public DateTime? CreateTime { get; set; }
        public int Type { get; set; }

        public int Status { get; set; }
        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
        public MaterialDic MaterialDic { get; set; }
        public OU OU { get; set; }

        public Order Order { get; set; }
        public OrderRow OrderRow { get; set; }
    }
}
