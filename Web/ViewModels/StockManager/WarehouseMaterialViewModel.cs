using System;
namespace Web.ViewModels.StockManager
{
    public class WarehouseMaterialViewModel
    {
        public int Id { get; set; }
        public int? SubOrderId { get; set; }
        public int? SubOrderRowId { get; set; }
        public string Code { get; set; }
        public int? MaterialDicId { get; set; }
        public string MaterialName { get; set; }
        public int? LocationId { get; set; }
        public string LocationCode { get; set; }
        public int MaterialCount { get; set; }
        public string WarehouseName { get; set; }
        public int? WarehouseTrayId { get; set; }
        public string TrayCode { get; set; }
        public int? ReservoirAreaId { get; set; }
        public string ReservoirAreaName { get; set; }
        public string CreateTime { get; set; }
        public string Carrier { get; set; }
        public string Img { get; set; }
        public int? WarehouseId { get; set; }
        public int? OUId { get; set; }
        public string OUName { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int? SupplierSiteId { get; set; }
        public string SupplierSiteName { get; set; }

        public string TrayStepStr { get; set; }

        public int? OutCount { get; set; }

        public string Spec { get; set; }
        
        public int? PhyWarehouseId { get; set; }
        
        public string PhyName { get; set; }

    }
}
