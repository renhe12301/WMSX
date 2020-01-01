using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class EmployeeRolePaginatedSpecification : BaseSpecification<EmployeeRole>
    {
        public EmployeeRolePaginatedSpecification(int skip,int take,int? roleId,
                                    int? employeeId,string roleName)
            : base(b => (!roleId.HasValue || b.SysRole.Id == roleId) &&
                  (!employeeId.HasValue || b.Employee.Id == employeeId) &&
                   (roleName==null||b.SysRole.RoleName==roleName))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Employee);
            AddInclude(b => b.SysRole);
        }
    }
}
