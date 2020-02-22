namespace Web.ViewModels.OrderManager
{
    public class OrderRowBatchViewModel
    {
        public int? OrderId { get; set; }
        public int? OrderRowId { get; set; }
        public int BatchCount { get; set; }
        public int ReservoirAreaId { get; set; }
        public int Type { get; set; }

        public object Tag { get; set; }
    }
}