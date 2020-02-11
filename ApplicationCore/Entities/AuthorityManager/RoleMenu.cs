using System;
namespace ApplicationCore.Entities.AuthorityManager
{
    public class RoleMenu:BaseEntity
    {
       public int SysRoleId { get; set; }
       public int SysMenuId { get; set; }
       public SysRole SysRole { get; set; }
       public SysMenu SysMenu { get; set; }
    }
}
