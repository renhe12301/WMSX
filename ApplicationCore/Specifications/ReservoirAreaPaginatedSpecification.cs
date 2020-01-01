using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class ReservoirAreaPaginatedSpecification:BaseSpecification<ReservoirArea>
    {
        public ReservoirAreaPaginatedSpecification(int skip, int take, int? id,int? sourceId,int? whId,string areaName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (!sourceId.HasValue || b.SourceId == sourceId) &&
                     (!whId.HasValue||b.WarehouseId==whId)&&
                     (areaName==null||b.AreaName==areaName))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Warehouse);
        }
    }
}
