using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class EmployeePaginatedSpecification:BaseSpecification<Employee>
    {
        public EmployeePaginatedSpecification(int skip,int take,int? id,string userName)
            : base(b =>(!id.HasValue || b.Id == id) &&
                  (userName == null || b.UserName == userName))
        {
            ApplyPaging(skip, take);
        }
    }
}
