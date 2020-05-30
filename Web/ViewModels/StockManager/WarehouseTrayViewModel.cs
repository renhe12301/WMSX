using System;
using System.Collections.Generic;

namespace Web.ViewModels.StockManager
{
    public class WarehouseTrayViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public string TrayCode { get; set; }
        public int LocationId { get; set; }

        public string LocationCode { get; set; }
        public int MaterialCount { get; set; }
        public int OutCount { get; set; }
        public string WarehouseName { get; set; }
        public string TrayStep { get; set; }
        public string ReservoirAreaName { get; set; }
        public string CreateTime { get; set; }
        public string Carrier { get; set; }
        public string OUName { get; set; }
        public int WarehouseId { get; set; }
        public int ReservoirId { get; set; }
        
        public  int CargoHeight { get; set; }

        public string CargoWeight { get; set; }

        public List<WarehouseMaterialViewModel> WarehouseMaterialViewModels { get; set; }
    }
}
