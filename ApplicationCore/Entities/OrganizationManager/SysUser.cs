using System;
namespace ApplicationCore.Entities.OrganizationManager
{
    /// <summary>
    /// 系统用户实体类
    /// </summary>
    public class SysUser : BaseEntity
    {
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
