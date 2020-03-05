using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;
namespace ApplicationCore.Specifications
{
    public class OrderRowSpecification: BaseSpecification<OrderRow>
    {
        public OrderRowSpecification(int? id,int? orderId,string orderNumber,
              List<int> status,string sCreateTime, string eCreateTime,
             string sFinishTime, string eFinishTime)
            :base(b => (!id.HasValue || b.Id == id) &&
                  (!orderId.HasValue||b.OrderId==orderId)&&
                  (status == null || status.Contains(b.Status)) &&
                  (orderNumber == null || b.Order.OrderNumber==orderNumber) &&
                  (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                  (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)) &&
                  (sFinishTime == null || b.FinishTime >= DateTime.Parse(sFinishTime)) &&
                  (eFinishTime == null || b.FinishTime <= DateTime.Parse(eFinishTime)))
        {
            AddInclude(b => b.Order);
            AddInclude(b=>b.MaterialDic);
            AddInclude(b=>b.ReservoirArea);
            AddInclude(b=>b.EBSTask);
        }
    }
}
