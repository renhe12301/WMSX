using System;
using ApplicationCore.Entities.OrganizationManager;
namespace ApplicationCore.Specifications
{
    public class OUSpecification:BaseSpecification<OU>
    {
        public OUSpecification(int?id,string name,string code,int? orgId):
            base(b=>(!id.HasValue||b.Id==id)&&
                    (name==null||b.OUName==name)&&
                    (code==null||b.OUCode==code)&&
                    (!orgId.HasValue)||b.OrganizationId==orgId)
        {
            AddInclude(b => b.Organization);
        }
    }
}
