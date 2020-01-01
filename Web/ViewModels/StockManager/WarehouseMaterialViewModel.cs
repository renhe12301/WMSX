using System;
namespace Web.ViewModels.StockManager
{
    public class WarehouseMaterialViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public string Code { get; set; }
        public string MaterialName { get; set; }
        public int LocationId { get; set; }
        public int MaterialCount { get; set; }
        public string WarehouseName { get; set; }
        public string TrayCode { get; set; }
        public string ReservoirAreaName { get; set; }
        public string CreateTime { get; set; }
        public string Carrier { get; set; }
        public string Img { get; set; }
    }
}
