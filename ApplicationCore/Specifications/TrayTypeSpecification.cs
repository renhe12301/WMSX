using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class TrayTypeSpecification: BaseSpecification<TrayType>
    {
        public TrayTypeSpecification(int? id,int? parentId,string typeName)
            :base(b=>!id.HasValue||b.Id==id&&
                  (typeName==null||b.TypeName==typeName)&&
                  (!parentId.HasValue||b.ParentId==parentId))
        {
            
        }
    }
}
