using System;
using System.Collections.Generic;

namespace Web.ViewModels.BasicInformation
{
    public class LocationViewModel
    {
        public int Id { get; set; }
        public string SysCode { get; set; }
        public string UserCode { get; set; }
        public string CreateTime { get; set; }
        public string Status { get; set; }
        public string InStock { get; set; }
        public string IsTask { get; set; }
        public string Memo { get; set; }
        public int WarehouseId { get; set; }
        public int ReservoirAreaId { get; set; }
        public int OUId { get; set; }
        public string OUName { get; set; }
        public int OrganizationId { get; set; }
        public string OrgName { get; set; }
        public string WarehouseName { get; set; }
      
        public string ReservoirAreaName { get; set;  }
        
        public int Row { get; set; }
        public int Rank { get; set; }
        public int Col { get; set; }
        public List<int> LocationIds { get; set; }
    }

}
