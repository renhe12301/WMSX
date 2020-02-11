using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class MaterialDicPaginatedSpecification : BaseSpecification<MaterialDic>
    {
        public MaterialDicPaginatedSpecification(int skip,int take, int? id, string materialCode, string materialName,
                                        int? typeId,string spec)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (materialCode == null || b.MaterialCode == materialCode) &&
                        (materialName == null || b.MaterialName.Contains(materialName)) &&
                        (!typeId.HasValue || b.MaterialTypeId == typeId) &&
                        (spec == null || b.Spec.Contains(spec)))
        {
            ApplyPaging(skip, take);
        }
    }
}
