using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.BasicInformation
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string Sex { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
        public string OrgName { get; set; }
        public int OrgId { get; set; }
        public string RoleName { get; set; }
        public string Img { get; set; }
        public object Tag { get; set; }
        public List<int> UserIds { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
