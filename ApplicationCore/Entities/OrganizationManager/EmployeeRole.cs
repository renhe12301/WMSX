using System;
namespace ApplicationCore.Entities.OrganizationManager
{
    public class EmployeeRole : BaseEntity
    {
        public int SysRoleId { get; set; }
        public int EmployeeId { get; set; }

        public SysRole SysRole { get; set; }
        public Employee Employee { get; set; }
    }
}
