using System;
namespace ApplicationCore.Entities.BasicInformation
{
    public class MaterialDicTypeArea: BaseEntity
    {
        public int WarehouseId { get; set; }
        public int ReservoirAreaId { get; set; }
        public int MaterialTypeId { get; set; }

        public Warehouse Warehouse { get; set; }
        public ReservoirArea ReservoirArea { get; set; }
        public MaterialType MaterialType { get; set; }
    }
}
