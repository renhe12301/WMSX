namespace ApplicationCore.Entities.BasicInformation
{
    public class SupplierSite:BaseEntity
    {
        public string SiteName { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string Contact { get; set; }
        public string TelPhone { get; set; }
        public int OUId { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public OU OU { get; set; }

    }
}