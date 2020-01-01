using System;
namespace Web.ViewModels.BasicInformation
{
    public class WarehouseViewModel
    {
        public int Id { get; set; }
        public int? SourceId { get; set; }
        public string WhName { get; set; }
        public string CreateTime { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }
        public int OrgId { get; set; }
        public string OrgName{get;set;}
    }
}
