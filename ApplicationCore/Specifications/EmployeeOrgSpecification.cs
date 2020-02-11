using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EmployeeOrgSpecification:BaseSpecification<EmployeeOrg>
    {
        public  EmployeeOrgSpecification(int? orgId,int? employeeId,string employeeName)
            : base(b => (!orgId.HasValue || b.OrganizationId == orgId) &&
                  (!employeeId.HasValue || b.EmployeeId == employeeId)&&
                  (employeeName==null||b.Employee.UserName==employeeName))
        {
            AddInclude(b => b.Employee);
            AddInclude(b => b.Organization);
        }
    }
}
