using System;
namespace Web.ViewModels.BasicInformation
{
    public class SupplierViewModel
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string TaxpayerCode { get; set; }
        public string CreateTime { get; set; }
        public string EndTime { get; set; }
        public string Memo { get; set; }
    }
}
