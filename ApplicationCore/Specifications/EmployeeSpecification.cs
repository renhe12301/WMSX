using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EmployeeSpecification:BaseSpecification<Employee>
    {
        public EmployeeSpecification(int? id,int? orgId,string userName)
            : base(b =>(!id.HasValue || b.Id == id) &&
                       (!orgId.HasValue || b.OrganizationId == orgId)&&
                       (userName == null || b.UserName.Contains(userName)))
        {
           
        }
    }
}
