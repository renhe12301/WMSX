using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels.WCSManager
{
    public class TaskReportViewModel
    {
        [BindProperty(Name = "taskId")]
        public string TaskId { get; set; }
        
        [BindProperty(Name = "reportTime")]
        public long ReportTime { get; set; }
        
        [BindProperty(Name = "taskStatus")]
        public string TaskStatus { get; set; }
        
        [BindProperty(Name = "returnInfo")]
        public string ReturnInfo { get; set; }
    }
}