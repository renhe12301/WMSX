using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class OUIdSetSpecification:BaseSpecification<OU>
    {
        public OUIdSetSpecification(List<int> ids)
            : base(b =>(ids == null || ids.Contains(b.Id)))
        {
        }
    }
}
