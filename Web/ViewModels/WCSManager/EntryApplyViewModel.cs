using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels.WCSManager
{
    public class EntryApplyViewModel
    {
        [BindProperty(Name = "applyTime")]
        public long ApplyTime { get; set; }
        [BindProperty(Name = "fromPort")]
        public string FromPort { get; set; }
        [BindProperty(Name = "barCode")]
        public string BarCode { get; set; }
        [BindProperty(Name = "cargoHeight")]
        public int CargoHeight { get; set; }
        [BindProperty(Name = "cargoWeight")]
        public string CargoWeight { get; set; }
    }
}