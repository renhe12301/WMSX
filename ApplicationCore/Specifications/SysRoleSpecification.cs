using System;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Specifications
{
    public class SysRoleSpecification:BaseSpecification<SysRole>
    {
        public SysRoleSpecification(int? id,int? parentId,string roleName)
            : base(b => (!id.HasValue || b.Id == id) &&
                   (roleName==null||b.RoleName==roleName)&&
                   (!parentId.HasValue || b.ParentId == parentId))
        {
            
        }
    }
}
