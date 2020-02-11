using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class OUSpecification:BaseSpecification<OU>
    {
        public OUSpecification(int?id,string name,string code):
            base(b=>(!id.HasValue||b.Id==id)&&
                    (name==null||b.OUName==name)&&
                    (code==null||b.OUCode==code))
        {
        }
    }
}
