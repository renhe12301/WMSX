using System;
namespace Web.ViewModels.OrganizationManager
{
    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Code { get; set; }
        public string OrgName { get; set; }
        public string Address { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
