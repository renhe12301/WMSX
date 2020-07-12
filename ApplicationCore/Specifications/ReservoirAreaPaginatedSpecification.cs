using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class ReservoirAreaPaginatedSpecification:BaseSpecification<ReservoirArea>
    {
        public ReservoirAreaPaginatedSpecification(int skip, int take, int? id, int? ouId, int? whId, string ownerType, string areaName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (!ouId.HasValue || b.OUId == ouId) &&
                     (!whId.HasValue||b.WarehouseId==whId)&&
                     (areaName==null||b.AreaName.Contains(areaName))&&
                     (ownerType == null || b.OwnerType == ownerType)
                  )
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Warehouse);
            // AddInclude(b=>b.OU);
            AddInclude(b=>b.PhyWarehouse);
        }
    }
}
