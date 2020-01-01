using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class WarehouseSpecification:BaseSpecification<Warehouse>
    {
        public WarehouseSpecification(int? id,int? sourceId,int? orgId,string whName)
            :base(b=>(!id.HasValue|| b.Id==id)&&
                  (!sourceId.HasValue || b.SourceId == sourceId)&&
                  (!orgId.HasValue||b.OrgId==orgId)&&
                   whName==null||b.WhName==whName)
        {
            AddInclude(b => b.Organization);
        }
    }
}
