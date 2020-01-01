using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 供应商实体类
    /// </summary>
    public class Supplier : BaseEntity
    {
        public string SupplierName { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Telephone { get; set; }
    }
}
