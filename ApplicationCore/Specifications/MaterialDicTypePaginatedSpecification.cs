using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class MaterialDicTypePaginatedSpecification: BaseSpecification<MaterialDicType>
    {
        public MaterialDicTypePaginatedSpecification(int take,int skip,int? dicId,int? typeId,
                                            string materialCode, string materialName,
                                            string spec, int? unitId, int? upLimit,
                                            int? downLimit)
           :base(b =>   (!dicId.HasValue || b.MaterialDic.Id == dicId) &&
                        (!typeId.HasValue || b.MaterialType.Id == dicId) &&
                        (materialCode == null || b.MaterialDic.MaterialCode == materialCode) &&
                        (materialName == null||b.MaterialDic.MaterialName==materialName)&&
                        (spec==null||b.MaterialDic.Spec==spec)&&
                        (!unitId.HasValue || b.MaterialDic.MaterialUnit.Id == unitId))
        {
            ApplyPaging(take, skip);
            AddInclude(b => b.MaterialDic);
            AddInclude(b => b.MaterialType);
            AddInclude($"{nameof(MaterialDicType.MaterialDic)}.{nameof(MaterialDic.MaterialUnit)}");
        }
    }
}
