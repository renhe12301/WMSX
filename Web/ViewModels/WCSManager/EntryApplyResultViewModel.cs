using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels.WCSManager
{
    public class EntryApplyResultViewModel
    {
        public int returnStatus { get; set; }
        public string returnInfo { get; set; }
        public long msgTime { get; set; }
    }
}