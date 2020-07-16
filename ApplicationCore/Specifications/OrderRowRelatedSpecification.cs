using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;
namespace ApplicationCore.Specifications
{
    public class OrderRowRelatedSpecification : BaseSpecification<OrderRow>
    {
        public OrderRowRelatedSpecification(List<int> orderTypeIds,int? related)
            :base(b =>(orderTypeIds==null||orderTypeIds.Contains(b.Order.OrderTypeId))&&
                  (!related.HasValue||b.RelatedId== related))
        {
        }
    }
}
