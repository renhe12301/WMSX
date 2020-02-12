using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class SupplierSitePaginatedSpecification : BaseSpecification<SupplierSite>
    {
        public SupplierSitePaginatedSpecification(int skip,int take,int? id, string supplierName,int? supplierId,int? ouId)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (supplierName == null || b.Supplier.SupplierName.Contains(supplierName))&&
                        (!supplierId.HasValue || b.SupplierId==supplierId)&&
                        (!ouId.HasValue || b.OUId == ouId))
        {
            ApplyPaging(skip, take);
            AddInclude(b=>b.OU);
            AddInclude(b=>b.Supplier);
        }
    }
}
