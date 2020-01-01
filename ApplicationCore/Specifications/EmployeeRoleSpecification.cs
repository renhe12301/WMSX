using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class EmployeeRoleSpecification:BaseSpecification<EmployeeRole>
    {
        public  EmployeeRoleSpecification(int? roleId,int? employeeId,string roleName)
            : base(b => (!roleId.HasValue || b.SysRole.Id == roleId) &&
                  (!employeeId.HasValue || b.Employee.Id == employeeId) &&
                   (roleName==null||b.SysRole.RoleName==roleName))
        {
            AddInclude(b => b.Employee);
            AddInclude(b => b.SysRole);
        }
    }
}
