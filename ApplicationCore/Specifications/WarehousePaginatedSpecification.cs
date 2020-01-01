using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class WarehousePaginatedSpecification : BaseSpecification<Warehouse>
    {
        public WarehousePaginatedSpecification(int skip, int take, int? id,int? sourceId,int? orgId,string whName)
            :base(b=>(!id.HasValue|| b.Id==id)&&
                  (!sourceId.HasValue || b.SourceId == sourceId)&&
                  (!orgId.HasValue||b.OrgId==orgId)&&
                   whName==null||b.WhName==whName)
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Organization);
        }
    }
}
