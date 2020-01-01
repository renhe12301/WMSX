using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class SupplierPaginatedSpecification : BaseSpecification<Supplier>
    {
        public SupplierPaginatedSpecification(int skip,int take,int? id, string supplierName)
             : base(b => (!id.HasValue || b.Id == id) &&
                    (supplierName == null || b.SupplierName == supplierName))
        {
            ApplyPaging(skip, take);
        }
    }
}
