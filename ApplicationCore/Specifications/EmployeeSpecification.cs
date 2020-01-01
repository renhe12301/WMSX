using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class EmployeeSpecification:BaseSpecification<Employee>
    {
        public EmployeeSpecification(int? id,string userName)
            : base(b =>(!id.HasValue || b.Id == id) &&
                  (userName == null || b.UserName == userName))
        {
           
        }
    }
}
