using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class MaterialTypeSpecification: BaseSpecification<MaterialType>
    {
        public MaterialTypeSpecification(int? id,int? parentId,string typeName)
            :base(b=>!id.HasValue||b.Id==id&&
                  (typeName==null||b.TypeName==typeName)&&
                  (!parentId.HasValue||b.ParentId==parentId))
        {
            
        }
    }
}
