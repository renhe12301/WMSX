using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Entities.BasicInformation
{
    public class OrderRowBatch:BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public int BatchCount { get; set; }
        public int IsRead { get; set; }
        public Order Order { get; set; }
        public OrderRow OrderRow { get; set; }
        
    }
}