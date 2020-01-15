using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class EmployeeOrgPaginatedSpecification:BaseSpecification<EmployeeOrg>
    {
        public  EmployeeOrgPaginatedSpecification(int skip,int take,int? orgId,int? employeeId,string employeeName)
            : base(b => (!orgId.HasValue || b.OrganizationId == orgId) &&
                  (!employeeId.HasValue || b.EmployeeId == employeeId)&&
                  (employeeName==null||b.Employee.UserName==employeeName))
        {
            ApplyPaging(skip,take);
            AddInclude(b => b.Employee);
            AddInclude(b => b.Organization);
        }
    }
}
