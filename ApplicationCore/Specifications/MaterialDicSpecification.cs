using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class MaterialDicSpecification : BaseSpecification<MaterialDic>
    {
        public MaterialDicSpecification(int? id, string materialCode, string materialName,
                                        int? typeId,string spec)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (materialCode == null || b.MaterialCode == materialCode) &&
                        (materialName == null||b.MaterialName.Contains(materialName))&&
                        (!typeId.HasValue || b.MaterialTypeId == typeId)&&
                        (spec==null||b.Spec.Contains(spec)))
        {

        }
    }
}
