using System;
namespace ApplicationCore.Entities.BasicInformation
{
    public class MaterialDicTypeArea: BaseEntity
    {
        public int WarehouseId { get; set; }
        public int ReservoidAreaId { get; set; }
        public int MaterialTypeId { get; set; }

        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
        public MaterialType MaterialType { get; set; }
    }
}
