using System;
using System.Collections.Generic;

namespace Web.ViewModels.StockManager
{
    public class WarehouseTrayViewModel
    {
        public int Id { get; set; }
        public int? SubOrderId { get; set; }
        public int? SubOrderRowId { get; set; }
        public string TrayCode { get; set; }
        public int? LocationId { get; set; }

        public string LocationCode { get; set; }
        public double MaterialCount { get; set; }
        public double OutCount { get; set; }
        public string WarehouseName { get; set; }
        public int? TrayStep { get; set; }
        public string ReservoirAreaName { get; set; }
        public string CreateTime { get; set; }
        public string Carrier { get; set; }
        public int? OUId { get; set; }
        public string OUName { get; set; }
        public int? WarehouseId { get; set; }
        public int? ReservoirAreaId { get; set; }
        
        public  int? CargoHeight { get; set; }

        public string CargoWeight { get; set; }

        public string TrayStepStr { get; set; }
        
        public int? PhyWarehouseId { get; set; }
        
        public string PhyName { get; set; }

        public List<WarehouseMaterialViewModel> WarehouseMaterialViewModels { get; set; }
    }
}
