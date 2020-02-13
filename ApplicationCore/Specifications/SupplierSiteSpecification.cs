using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class SupplierSiteSpecification:BaseSpecification<SupplierSite>
    {
        public SupplierSiteSpecification(int? id, string supplierName,int? supplierId,int? ouId)
             : base(b => (!id.HasValue || b.Id == id) &&
                    (supplierName == null || b.Supplier.SupplierName.Contains(supplierName))&&
                    (!supplierId.HasValue || b.SupplierId==supplierId)&&
                    (!ouId.HasValue || b.OUId == ouId))
        {
            AddInclude(b=>b.OU);
            AddInclude(b=>b.Supplier);
        }
    }
}
