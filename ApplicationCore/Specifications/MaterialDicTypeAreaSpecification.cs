using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class MaterialDicTypeAreaSpecification : BaseSpecification<MaterialDicTypeArea>
    {
        public MaterialDicTypeAreaSpecification(int? typeId,int? areaId,int? warehouseId)
            :base(b=>(!typeId.HasValue || b.MaterialType.Id==typeId)&&
                       (!areaId.HasValue || b.ReservoirArea.Id == areaId)&&
                       (!warehouseId.HasValue || b.Warehouse.Id == warehouseId) )
        {
            AddInclude(b => b.MaterialType);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.Warehouse);
        }
    }
}
