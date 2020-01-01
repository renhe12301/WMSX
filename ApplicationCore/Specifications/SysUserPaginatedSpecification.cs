using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class SysUserPaginatedSpecification:BaseSpecification<SysUser>
    {
        public SysUserPaginatedSpecification(int skip,int take,int? id, int? employeeId,string loginName)
            : base(b => (!id.HasValue || b.Id == id) &&
                      (!employeeId.HasValue || b.EmployeeId == employeeId)&&
                      (loginName==null||b.LoginName==loginName))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Employee);
        }
    }
}
