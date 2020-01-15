using System;
using ApplicationCore.Entities.OrganizationManager;
namespace ApplicationCore.Specifications
{
    public class OUPaginatedSpecification:BaseSpecification<OU>
    {
        public OUPaginatedSpecification(int skip,int take,int?id,string name,string code,int? orgId):
            base(b=>(!id.HasValue||b.Id==id)&&
                    (name==null||b.OUName==name)&&
                    (code==null||b.OUCode==code)&&
                    (!orgId.HasValue)||b.OrganizationId==orgId)
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Organization);
        }
    }
}
