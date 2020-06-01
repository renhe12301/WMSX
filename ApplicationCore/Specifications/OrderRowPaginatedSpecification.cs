using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;
namespace ApplicationCore.Specifications
{
    public class OrderRowPaginatedSpecification: BaseSpecification<OrderRow>
    {
        public OrderRowPaginatedSpecification(int skip,int take,int? id,int? orderId,
            List<int> orderTypeIds,int? sourceId,string orderNumber,
            int? ouId,int? warehouseId,int? supplierId, string supplierName,int? supplierSiteId, string supplierSiteName,
            List<int> status,string sCreateTime, string eCreateTime,
             string sFinishTime, string eFinishTime)
            :base(b => (!id.HasValue || b.Id == id) &&
                  (!orderId.HasValue||b.OrderId==orderId)&&
                  (orderTypeIds == null || orderTypeIds.Contains(b.Order.OrderTypeId)) &&
                  (!sourceId.HasValue||b.SourceId==sourceId)&&
                  (orderNumber == null || b.Order.OrderNumber==orderNumber) &&
                  (!ouId.HasValue||b.Order.OUId==ouId)&&
                  (!warehouseId.HasValue||b.Order.WarehouseId==warehouseId)&&
                  (!supplierId.HasValue||b.Order.SupplierId==supplierId)&&
                  (supplierName == null || b.Order.Supplier.SupplierName.Contains(supplierName)) &&
                  (!supplierSiteId.HasValue||b.Order.SupplierSiteId==supplierSiteId)&&
                  (supplierSiteName == null || b.Order.SupplierSite.SiteName.Contains(supplierSiteName)) &&
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
