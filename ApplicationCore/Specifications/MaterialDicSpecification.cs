using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class MaterialDicSpecification : BaseSpecification<MaterialDic>
    {
        public MaterialDicSpecification(int? id, string materialCode, string materialName,
                                        string spec,int? unitId)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (materialCode == null || b.MaterialCode == materialCode) &&
                        (materialName == null||b.MaterialName==materialName)&&
                        (spec==null||b.Spec==spec)&&
                        (!unitId.HasValue || b.UnitId == unitId))
        {
            AddInclude(b => b.MaterialUnit);
        }
    }
}
