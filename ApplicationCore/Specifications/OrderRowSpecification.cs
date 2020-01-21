using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;
namespace ApplicationCore.Specifications
{
    public class OrderRowSpecification: BaseSpecification<OrderRow>
    {
        public OrderRowSpecification(int? id,int? orderId,List<int> warehouseIds,
             List<int> progressRange,string sCreateTime, string eCreateTime,
             string sFinishTime, string eFinishTime)
            :base(b => (!id.HasValue || b.Id == id) &&
                  (!orderId.HasValue||b.OrderId==orderId)&&
                  (warehouseIds ==null)&&
                  (progressRange == null || (b.Progress >= progressRange[0] && b.Progress <= progressRange[1])) &&
                  (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                  (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)) &&
                  (sFinishTime == null || b.FinishTime >= DateTime.Parse(sFinishTime)) &&
                  (eFinishTime == null || b.FinishTime <= DateTime.Parse(eFinishTime)))
        {
            AddInclude(b => b.Order);
        }
    }
}
