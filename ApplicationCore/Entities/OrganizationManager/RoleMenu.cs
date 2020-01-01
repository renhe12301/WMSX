using System;
namespace ApplicationCore.Entities.OrganizationManager
{
    /// <summary>
    /// 角色对应菜单实体类
    /// </summary>
    public class RoleMenu:BaseEntity
    {
       public int RoleId { get; set; }
       public int MenuId { get; set; }
       public SysRole SysRole { get; set; }
       public SysMenu SysMenu { get; set; }
    }
}
