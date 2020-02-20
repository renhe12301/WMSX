using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Specifications
{
    public class OrderRowBatchSpecification:BaseSpecification<OrderRowBatch>
    
    {
        public OrderRowBatchSpecification(int? isRead)
            :base(b=>(!isRead.HasValue||b.IsRead==isRead))
        {
            
        }
    }
}