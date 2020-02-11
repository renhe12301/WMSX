using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 供应商实体类
    /// </summary>
    public class Supplier : BaseEntity
    {
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string TaxpayerCode { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Memo { get; set; }
    }
}
