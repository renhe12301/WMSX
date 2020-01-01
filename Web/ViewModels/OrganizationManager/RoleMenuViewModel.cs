﻿using System;
using System.Collections.Generic;

namespace Web.ViewModels.OrganizationManager
{
    public class RoleMenuViewModel
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public string RoleName { get; set; }
        public string MenuName { get; set; }
        public List<int> MenuIds = new List<int>();
    }
}
