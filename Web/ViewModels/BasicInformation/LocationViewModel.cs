using System;
using System.Collections.Generic;

namespace Web.ViewModels.BasicInformation
{
    public class LocationViewModel
    {
        public int Id { get; set; }
        public string SysCode { get; set; }
        public string UserCode { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int IsLock { get; set; }
        public int InStock { get; set; }
        public string Memo { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
        public string ReservoirAreaName { get; set;  }
        public int ReservoirAreaId { get; set; }
        public int Row { get; set; }
        public int Rank { get; set; }
        public int Col { get; set; }
        public List<int> LocationIds { get; set; }
    }

}
