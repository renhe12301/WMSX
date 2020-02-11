using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class OrganizationSpecification:BaseSpecification<Organization>
    {
        public OrganizationSpecification(int? id,string orgCode,int? ouid,string orgName)
            : base(b => (!id.HasValue || b.Id == id)&&
                        (orgCode==null || b.OrgCode == orgCode)&&
                  (!ouid.HasValue||b.OUId==ouid)&&
                  (orgName==null||b.OrgName==orgName))
        {
           
        }
    }
}
