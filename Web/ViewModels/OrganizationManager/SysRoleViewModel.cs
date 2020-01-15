using System;
using System.Collections.Generic;

namespace Web.ViewModels.OrganizationManager
{
    public class SysRoleViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string RoleName { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
        public string Status { get; set; }
        public List<SysMenuViewModel> SysMenuViewModels = new List<SysMenuViewModel>();
    }
}
