using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.OrganizationManager
{
    public class EmployeeRoleViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string OrgName { get; set; }
        public int OrganizationId { get; set; }
        public List<int> RoleIds = new List<int>();
    }
}
