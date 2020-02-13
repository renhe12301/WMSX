using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class ReservoirAreaIdSetSpecification:BaseSpecification<ReservoirArea>
    {
        public ReservoirAreaIdSetSpecification(List<int> ids)
            : base(b =>(ids == null || ids.Contains(b.Id)))
        {
            
        }
    }
}
