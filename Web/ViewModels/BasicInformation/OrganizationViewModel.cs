using System;
namespace Web.ViewModels.BasicInformation
{
    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public string OUName { get; set; }
        public string OrgName { get; set; }
        public string OrgCode { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
