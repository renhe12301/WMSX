using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class SupplierIdSetSpecification:BaseSpecification<Supplier>
    {
        public SupplierIdSetSpecification(List<int> ids)
            : base(b =>(ids == null || ids.Contains(b.Id)))
        {

        }
    }
}
