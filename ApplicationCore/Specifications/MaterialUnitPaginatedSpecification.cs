using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class MaterialUnitPaginatedSpecification : BaseSpecification<MaterialUnit>
    {
        public MaterialUnitPaginatedSpecification(int pageIndex,int itemsPage,int? id, string unitName)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (unitName == null || b.UnitName == unitName))
        {
            ApplyPaging(pageIndex, itemsPage);
        }
    }
}
