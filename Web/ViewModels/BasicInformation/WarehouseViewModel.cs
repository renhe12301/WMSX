using System;
namespace Web.ViewModels.BasicInformation
{
    public class WarehouseViewModel
    {
        public int Id { get; set; }
        public string WhName { get; set; }
        public string WhCode { get; set; }
        public string CreateTime { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }
        public int OUId { get; set; }
        public string OUName { get; set; }
    }
}
