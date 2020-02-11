using System;
using ApplicationCore.Entities.AuthorityManager;

namespace ApplicationCore.Specifications
{
    public class EmployeeRolePaginatedSpecification : BaseSpecification<EmployeeRole>
    {
        public EmployeeRolePaginatedSpecification(int skip,int take,int? roleId,
                                    int? employeeId)
            : base(b => (!roleId.HasValue || b.SysRoleId == roleId) &&
                  (!employeeId.HasValue || b.EmployeeId == employeeId))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Employee);
            AddInclude(b => b.SysRole);
        }
    }
}
