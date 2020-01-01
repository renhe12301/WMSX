using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class MaterialUnitSpecification : BaseSpecification<MaterialUnit>
    {
        public MaterialUnitSpecification(int? id, string unitName)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (unitName == null || b.UnitName == unitName))
        {
        }
    }
}
