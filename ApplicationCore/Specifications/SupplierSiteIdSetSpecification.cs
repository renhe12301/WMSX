using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class SupplierSiteIdSetSpecification:BaseSpecification<SupplierSite>
    {
        public SupplierSiteIdSetSpecification(List<int> ids)
            : base(b =>(ids == null || ids.Contains(b.Id)))
        {
            
        }
    }
}
