using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels.WCSManager
{
    public class TaskReportResultViewModel
    {
        public string taskId { get; set; }
        public int returnStatus { get; set; }
        public string returnInfo { get; set; }
        public long msgTime { get; set; }
    }
}