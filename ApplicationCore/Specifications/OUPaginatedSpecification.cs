using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class OUPaginatedSpecification:BaseSpecification<OU>
    {
        public OUPaginatedSpecification(int skip,int take,int?id,string name,string code):
            base(b=>(!id.HasValue||b.Id==id)&&
                    (name==null||b.OUName==name)&&
                    (code==null||b.OUCode==code))
        {
            ApplyPaging(skip, take);
        }
    }
}
