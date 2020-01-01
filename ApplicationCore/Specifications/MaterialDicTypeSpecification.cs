using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class MaterialDicTypeSpecification: BaseSpecification<MaterialDicType>
    {
        public MaterialDicTypeSpecification(int? dicId,int? typeId,
                                            string materialCode, string materialName,
                                            string spec, int? unitId)
           :base(b =>   (!dicId.HasValue || b.MaterialDic.Id == dicId) &&
                        (!typeId.HasValue || b.MaterialType.Id == dicId) &&
                        (materialCode == null || b.MaterialDic.MaterialCode == materialCode) &&
                        (materialName == null||b.MaterialDic.MaterialName==materialName)&&
                        (spec==null||b.MaterialDic.Spec==spec)&&
                        (!unitId.HasValue || b.MaterialDic.MaterialUnit.Id == unitId))
        {
            AddInclude(b => b.MaterialDic);
            AddInclude(b => b.MaterialType);
            AddInclude($"{nameof(MaterialDicType.MaterialDic)}.{nameof(MaterialDic.MaterialUnit)}");
        }
    }
}
