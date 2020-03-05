namespace Web.ViewModels.OrderManager
{
    public class OrderRowBatchViewModel
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? OrderRowId { get; set; }
        public int BatchCount { get; set; }
        public int RealityCount { get; set; }
        public int ReservoirAreaId { get; set; }
        public int Type { get; set; }
        public string TypeStr { get; set; }

        public string Tag { get; set; }

        public int Status { get; set; }

        public string StatusStr { get; set; }
        
        public string IsSyncStr { get; set; }
        
        public string IsReadStr { get; set; }
    }
}