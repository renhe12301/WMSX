using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class SysUserSpecification:BaseSpecification<SysUser>
    {
        public SysUserSpecification(int? id, int? employeeId,string loginName)
            : base(b => (!id.HasValue || b.Id == id) &&
                      (!employeeId.HasValue || b.EmployeeId == employeeId)&&
                      (loginName==null||b.LoginName==loginName))
        {
            AddInclude(b => b.Employee);
        }
    }
}
