using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class TrayDicTypeAreaSpecification : BaseSpecification<TrayDicTypeArea>
    {
        public TrayDicTypeAreaSpecification(int? typeId,int? areaId,int? warehouseId)
            :base(b=>(!typeId.HasValue || b.TrayType.Id==typeId)&&
                       (!areaId.HasValue || b.ReservoirArea.Id == areaId)&&
                       (!warehouseId.HasValue || b.Warehouse.Id == warehouseId) )
        {
            AddInclude(b => b.TrayType);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.Warehouse);
        }
    }
}
