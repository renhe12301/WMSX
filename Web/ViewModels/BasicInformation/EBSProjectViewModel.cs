namespace Web.ViewModels.BasicInformation
{
    public class EBSProjectViewModel
    {
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectFullName { get; set; }
        public int OUId { get; set; }
        public string OUName { get; set; }
        public int OrganizationId { get; set; }
        public string OrgName { get; set; }
        public string TypeCode { get; set; }
        public string StatusCode { get; set; }
        public string ProjectCategory { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CreateTime { get; set; }
        public string EndTime { get; set; }
    }
}