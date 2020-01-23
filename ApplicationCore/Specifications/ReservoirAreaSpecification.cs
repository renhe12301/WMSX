using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class ReservoirAreaSpecification:BaseSpecification<ReservoirArea>
    {
        public ReservoirAreaSpecification(int? id, int? orgId, int? ouId,int? whId,int? type,string areaName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                      (!ouId.HasValue || b.OUId == ouId) &&
                      (!orgId.HasValue || b.OrganizationId == orgId) &&
                     (!whId.HasValue||b.WarehouseId==whId)&&
                     (!type.HasValue || b.Type == type) &&
                     (areaName==null||b.AreaName==areaName))
        {
            AddInclude(b => b.Warehouse);
            AddInclude(b=>b.OU);
            AddInclude(b=>b.Organization);
        }
    }
}
