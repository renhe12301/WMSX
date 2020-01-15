using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class WarehouseSpecification:BaseSpecification<Warehouse>
    {
        public WarehouseSpecification(int? id,int? orgId,int? ouId,string whName)
            :base(b=>(!id.HasValue|| b.Id==id)&&
                  (!ouId.HasValue || b.OUId == ouId)&&
                  (!orgId.HasValue||b.OrganizationId==orgId)&&
                   whName==null||b.WhName==whName)
        {
            AddInclude(b => b.Organization);
        }
    }
}
