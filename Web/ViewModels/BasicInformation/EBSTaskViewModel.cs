namespace Web.ViewModels.BasicInformation
{
    public class EBSTaskViewModel
    {
        public int Id { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public string Summary { get; set; }
        public string TaskLevel { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string CreateTime { get; set; }
        public string EndTime { get; set; }
    }
}