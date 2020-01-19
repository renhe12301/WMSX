using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class MaterialTypePaginatedSpecification: BaseSpecification<MaterialType>
    {
        public MaterialTypePaginatedSpecification(int skip,int take,int? id,int? parentId,string typeName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                  (typeName==null||b.TypeName==typeName)&&
                  (!parentId.HasValue||b.ParentId==parentId))
        {
            ApplyPaging(skip, take);
        }
    }
}
