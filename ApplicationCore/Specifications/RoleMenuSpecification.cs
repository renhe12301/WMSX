using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class RoleMenuSpecification:BaseSpecification<RoleMenu>
    {
        public RoleMenuSpecification(int? roleId)
            :base(b=>(!roleId.HasValue||b.SysRoleId==roleId))
        {
            AddInclude(b => b.SysMenu);
            AddInclude(b => b.SysRole);
        }
    }
}
