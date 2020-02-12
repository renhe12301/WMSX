using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class OUPaginatedSpecification:BaseSpecification<OU>
    {
        public OUPaginatedSpecification(int skip,int take,int?id,string name,string code,string companyName):
            base(b=>(!id.HasValue||b.Id==id)&&
                    (name==null||b.OUName.Contains(name))&&
                    (code==null||b.OUCode==code)&&
                    (companyName==null||b.CompanyName.Contains(companyName)))
        {
            ApplyPaging(skip, take);
        }
    }
}
