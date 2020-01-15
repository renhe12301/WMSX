using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class SysRolePaginatedSpecification:BaseSpecification<SysRole>
    {
        public SysRolePaginatedSpecification(int skip,int take,int? id,string roleName)
            : base(b => (!id.HasValue || b.Id == id) &&
                   (roleName==null||b.RoleName==roleName))
        {
            ApplyPaging(skip, take);
        }
    }
}
