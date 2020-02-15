using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EBSProjectIdSetSpecification:BaseSpecification<Employee>
    {
        public EBSProjectIdSetSpecification(List<int> ids)
            : base(b =>(ids == null || ids.Contains(b.Id)))
        {
           
        }
    }
}