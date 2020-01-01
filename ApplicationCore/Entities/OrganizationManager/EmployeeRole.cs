using System;
namespace ApplicationCore.Entities.OrganizationManager
{
    public class EmployeeRole : BaseEntity
    {
        public int RoleId { get; set; }
        public int EmployeeId { get; set; }

        public SysRole SysRole;
        public Employee Employee;
    }
}
