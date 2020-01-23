using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class WarehousePaginatedSpecification : BaseSpecification<Warehouse>
    {
        public WarehousePaginatedSpecification(int skip, int take, int? id,int? orgId,int? ouId,string whName)
            :base(b=>(!id.HasValue|| b.Id==id)&&
                  (!orgId.HasValue||b.OrganizationId==orgId)&&
                   (!ouId.HasValue || b.OUId == ouId) &&
                   whName ==null||b.WhName.Contains(whName))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Organization);
        }
    }
}
