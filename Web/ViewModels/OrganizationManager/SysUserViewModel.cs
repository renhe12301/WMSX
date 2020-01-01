using System;
namespace Web.ViewModels.OrganizationManager
{
    public class SysUserViewModel
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public string Status { get; set; }
        public string CreateTime { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }
}
