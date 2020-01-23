using System;
namespace Web.ViewModels.BasicInformation
{
    public class ReservoirAreaViewModel
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public string AreaName { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
        public int OUId { get; set; }
        public string OUName { get; set; }
        public int OrganizationId { get; set; }
        public string OrgName { get; set; }
        public string CreateTime { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public string Memo { get; set; }
    }
}
