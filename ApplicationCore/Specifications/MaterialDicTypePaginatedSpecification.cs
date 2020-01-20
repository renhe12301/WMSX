using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class MaterialDicTypePaginatedSpecification: BaseSpecification<MaterialDicType>
    {
        public MaterialDicTypePaginatedSpecification(int take,int skip,int? dicId,int? typeId,
                                            string materialCode, string materialName,
                                            string spec)
           :base(b =>   (!dicId.HasValue || b.MaterialDic.Id == dicId) &&
                        (!typeId.HasValue || b.MaterialType.Id == typeId) &&
                        (materialCode == null || b.MaterialDic.MaterialCode == materialCode) &&
                        (materialName == null||b.MaterialDic.MaterialName==materialName)&&
                        (spec==null||b.MaterialDic.Spec==spec))
        {
            ApplyPaging(take, skip);
            AddInclude(b => b.MaterialDic);
            AddInclude(b => b.MaterialType);
        }
    }
}
