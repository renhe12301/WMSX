using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class OrganizationSpecification:BaseSpecification<Organization>
    {
        public OrganizationSpecification(int? id,int? pid,string orgName)
            : base(b => (!id.HasValue || b.Id == id)&&
                  (!pid.HasValue||b.ParentId==pid)&&
                  (orgName==null||b.OrgName==orgName))
        {
           
        }
    }
}
