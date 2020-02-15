using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EBSTaskIdSetSpecification:BaseSpecification<Employee>
    {
        public EBSTaskIdSetSpecification(List<int> ids)
            : base(b =>(ids == null || ids.Contains(b.Id)))
        {
           
        }
    }
}