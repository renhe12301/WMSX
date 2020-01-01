using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class ReservoirAreaSpecification:BaseSpecification<ReservoirArea>
    {
        public ReservoirAreaSpecification(int? id,int? sourceId,int? whId,int? type,string areaName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (!sourceId.HasValue || b.SourceId == sourceId) &&
                     (!whId.HasValue||b.WarehouseId==whId)&&
                     (!type.HasValue || b.Type == type) &&
                     (areaName==null||b.AreaName==areaName))
        {
            AddInclude(b => b.Warehouse);
        }
    }
}
