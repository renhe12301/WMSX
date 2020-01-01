using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.OrganizationManager
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
        public int ParentId { get; set; }
        public int Type { get; set; }
    }
}
