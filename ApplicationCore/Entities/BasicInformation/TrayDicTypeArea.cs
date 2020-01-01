using System;
namespace ApplicationCore.Entities.BasicInformation
{
    public class TrayDicTypeArea:BaseEntity
    {
        public int TrayTypeId { get; set; }
        public int WarehouseId { get; set; }
        public int ReservoirAreaId { get; set; }
        public TrayType TrayType { get; set; }
        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
    }
}
