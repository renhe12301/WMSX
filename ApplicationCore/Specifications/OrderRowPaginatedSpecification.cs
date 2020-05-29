using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;
namespace ApplicationCore.Specifications
{
    public class OrderRowPaginatedSpecification: BaseSpecification<OrderRow>
    {
        public OrderRowPaginatedSpecification(int skip,int take,int? id,int? orderId,
            int? orderType,int? sourceId,List<int> status,string sCreateTime, string eCreateTime,
             string sFinishTime, string eFinishTime)
            :base(b => (!id.HasValue || b.Id == id) &&
                  (!orderId.HasValue||b.OrderId==orderId)&&
                  (!orderType.HasValue||b.Order.OrderTypeId==orderType)&&
                  (!sourceId.HasValue||b.SourceId==sourceId)&&
                  (status == null || status.Contains(b.Status.GetValueOrDefault())) &&
                  (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                  (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)) &&
                  (sFinishTime == null || b.FinishTime >= DateTime.Parse(sFinishTime)) &&
                  (eFinishTime == null || b.FinishTime <= DateTime.Parse(eFinishTime)))
        {
            ApplyPaging(skip,take);
            AddInclude(b => b.Order);
            AddInclude(b=>b.MaterialDic);
            AddInclude(b=>b.ReservoirArea);
            AddInclude(b=>b.EBSTask);
        }
    }
}
