using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class MaterialDicTypeSpecification: BaseSpecification<MaterialDicType>
    {
        public MaterialDicTypeSpecification(int? dicId,int? typeId,
                                            string materialCode, string materialName,
                                            string spec)
           :base(b =>   (!dicId.HasValue || b.MaterialDic.Id == dicId) &&
                        (!typeId.HasValue || b.MaterialType.Id == typeId) &&
                        (materialCode == null || b.MaterialDic.MaterialCode == materialCode) &&
                        (materialName == null||b.MaterialDic.MaterialName.Contains(materialName))&&
                        (spec==null||b.MaterialDic.Spec.Contains(spec)))
        {
            AddInclude(b => b.MaterialDic);
            AddInclude(b => b.MaterialType);
        }
    }
}
