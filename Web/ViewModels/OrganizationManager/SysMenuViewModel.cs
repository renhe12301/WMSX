using System;
using System.Collections.Generic;
namespace Web.ViewModels.OrganizationManager
{
    public class SysMenuViewModel
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public string CreateTime { get; set; }
        public int ParentId { get; set; }
        public int IsLeaf { get; set; }
        public string Memo { get; set; }
    }
}
