using System;
using ApplicationCore.Entities.AuthorityManager;

namespace ApplicationCore.Specifications
{
    public class EmployeeRoleSpecification:BaseSpecification<EmployeeRole>
    {
        public  EmployeeRoleSpecification(int? roleId,int? employeeId,string employeeName)
            : base(b => (!roleId.HasValue || b.SysRoleId == roleId) &&
                  (!employeeId.HasValue || b.EmployeeId == employeeId)&&
                  (employeeName==null||b.Employee.UserName==employeeName))
        {
            AddInclude(b => b.Employee);
            AddInclude(b => b.SysRole);
        }
    }
}
