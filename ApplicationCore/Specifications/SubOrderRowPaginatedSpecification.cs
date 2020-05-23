using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;
namespace ApplicationCore.Specifications
{
    public class SubOrderRowPaginatedSpecification: BaseSpecification<SubOrderRow>
    {
        public SubOrderRowPaginatedSpecification(int skip,int take,int? id,int? orderId,int? orderType,int? orderRowId,string orderNumber,
               List<int> status,string sCreateTime, string eCreateTime,
               string sFinishTime, string eFinishTime)
            :base(b => (!id.HasValue || b.Id == id) &&
                  (!orderId.HasValue||b.SubOrderId==orderId)&&
                  (!orderType.HasValue||b.SubOrder.OrderTypeId==orderType)&&
                  (!orderRowId.HasValue||b.OrderRowId==orderRowId)&&
                  (status == null || status.Contains(b.Status.GetValueOrDefault())) &&
                  (orderNumber == null || b.SubOrder.OrderNumber==orderNumber) &&
                  (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                  (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)) &&
                  (sFinishTime == null || b.FinishTime >= DateTime.Parse(sFinishTime)) &&
                  (eFinishTime == null || b.FinishTime <= DateTime.Parse(eFinishTime)))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.SubOrder);
            AddInclude(b=>b.MaterialDic);
            AddInclude(b=>b.ReservoirArea);
            AddInclude(b=>b.OrderRow);
        }
    }
}
