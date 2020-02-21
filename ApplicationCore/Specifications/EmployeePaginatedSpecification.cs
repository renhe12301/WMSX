using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EmployeePaginatedSpecification:BaseSpecification<Employee>
    {
        public EmployeePaginatedSpecification(int skip,int take,int? id,int? orgId,string userName)
            : base(b =>(!id.HasValue || b.Id == id) &&
                       (!orgId.HasValue || b.OrganizationId == orgId) &&    
                       (userName == null || b.UserName.Contains(userName)))
        {
            ApplyPaging(skip, take);
        }
    }
}
