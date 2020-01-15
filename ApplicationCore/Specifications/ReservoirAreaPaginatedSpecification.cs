using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class ReservoirAreaPaginatedSpecification:BaseSpecification<ReservoirArea>
    {
        public ReservoirAreaPaginatedSpecification(int skip, int take, int? id, int? orgId, int? ouId, int? whId,string areaName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (!ouId.HasValue || b.OUId == ouId) &&
                     (!orgId.HasValue || b.OrganizationId == orgId) &&
                     (!whId.HasValue||b.WarehouseId==whId)&&
                     (areaName==null||b.AreaName==areaName))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Warehouse);
        }
    }
}
