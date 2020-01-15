using System;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Entities.FlowRecord
{
    public class InOutRecord:BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public int WarehouseId { get; set; }
        public int ReservoirAreaId { get; set; }
        public int? OrganizationId { get; set; }
        public int? OUId { get; set; }
        public int TrayDicId { get; set; }
        public int MaterialDicId { get; set; }
        public int InOutCount { get; set; }
        public int IsRead { get; set; }
        public DateTime CreateTime { get; set; }
        public int Type { get; set; }
        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
        public MaterialDic MaterialDic { get; set; }
        public TrayDic TrayDic { get; set; }
        public Organization Organization { get; set; }
        public OU OU { get; set; }
    }
}
