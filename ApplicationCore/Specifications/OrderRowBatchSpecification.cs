using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Specifications
{
    public class OrderRowBatchSpecification:BaseSpecification<OrderRowBatch>
    
    {
        public OrderRowBatchSpecification(int? id,int?orderId,int? orderRowId,
            int? areaId,int? isRead,int? type,int? isSync,List<int> status)
            :base(b=> (!id.HasValue||b.Id==id)&&
                                            (!orderId.HasValue||b.OrderId==orderId)&&
                                            (!orderRowId.HasValue||b.OrderRowId==orderRowId)&&
                                            (!areaId.HasValue||b.ReservoirAreaId==areaId)&&
                                            (!isRead.HasValue||b.IsRead==isRead)&&
                                            (!type.HasValue||b.Type==type)&&
                                            (!isSync.HasValue||b.IsSync==isSync)&&
                                            (status==null||status.Contains(b.Status))&&
                                            (!isRead.HasValue||b.IsRead==isRead))
        {
            AddInclude(b=>b.Order);
            AddInclude(b=>b.OrderRow);
            AddInclude(b=>b.ReservoirArea);
        }
    }
}