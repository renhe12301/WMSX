using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class SupplierSpecification:BaseSpecification<Supplier>
    {
        public SupplierSpecification(int? id, string supplierName)
             : base(b => (!id.HasValue || b.Id == id) &&
                    (supplierName == null || b.SupplierName == supplierName))
        {

        }
    }
}
