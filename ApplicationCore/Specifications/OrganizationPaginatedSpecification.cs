using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class OrganizationPaginatedSpecification:BaseSpecification<Organization>
    {
        public OrganizationPaginatedSpecification(int skip,int take,int? id,int? ouid,string orgName)
            : base(b => (!id.HasValue || b.Id == id)&&
                  (!ouid.HasValue||b.OUId==ouid)&&
                  (orgName==null||b.OrgName.Contains(orgName)))
        {
            ApplyPaging(skip, take);
            AddInclude(b=>b.OU);
        }
    }
}
