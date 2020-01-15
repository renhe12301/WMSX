using System;
namespace ApplicationCore.Entities.OrganizationManager
{
    /// <summary>
    /// 角色对应菜单实体类
    /// </summary>
    public class RoleMenu:BaseEntity
    {
       public int SysRoleId { get; set; }
       public int SysMenuId { get; set; }
       public SysRole SysRole { get; set; }
       public SysMenu SysMenu { get; set; }
    }
}
