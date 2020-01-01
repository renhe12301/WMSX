using System;
using System.Collections.Generic;
namespace ApplicationCore.Entities.OrganizationManager
{
    /// <summary>
    /// 系统菜单实体类
    /// </summary>
    public class SysMenu : BaseEntity
    {
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public DateTime CreateTime { get; set; }
        public int ParentId { get; set; }
        public int IsLeaf { get; set; }
        public string Memo { get; set; }
    }
}
