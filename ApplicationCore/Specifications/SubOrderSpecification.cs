using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Specifications
{
    public class SubOrderSpecification:BaseSpecification<SubOrder>
    {
        public SubOrderSpecification(int? id, string orderNumber, List<int> orderTypeIds,
             List<int> status,int? isRead,int? isSync, int? ouId, int? warehouseId, int? pyId,int? supplierId, string supplierName,
             int? supplierSiteId,string supplierSiteName,string sCreateTime, string eCreateTime,
             string sFinishTime,string eFinishTime)
            : base(b => (!id.HasValue || b.Id == id) &&
                       (orderNumber == null || b.OrderNumber.Contains(orderNumber)) &&
                       (orderTypeIds == null || orderTypeIds.Contains(b.OrderTypeId)) &&
                       (status == null || status.Contains(b.Status.GetValueOrDefault())) &&
                       (!isRead.HasValue || b.IsRead == isRead) &&
                       (!isSync.HasValue || b.IsSync == isSync) &&
                       (!ouId.HasValue || b.OUId == ouId) &&
                       (!warehouseId.HasValue || b.WarehouseId == pyId) &&
                       (!pyId.HasValue || b.PhyWarehouseId == pyId) &&
                       (!supplierId.HasValue || b.SupplierId == supplierId) &&
                       (supplierName == null || b.Supplier.SupplierName.Contains(supplierName)) &&
                       (!supplierSiteId.HasValue || b.SupplierSiteId == supplierSiteId) &&
                       (supplierSiteName == null || b.SupplierSite.SiteName.Contains(supplierSiteName)) &&
                       (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                       (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime))&&
                       (sFinishTime == null || b.FinishTime >= DateTime.Parse(sFinishTime)) &&
                       (eFinishTime == null || b.FinishTime <= DateTime.Parse(eFinishTime)))
        {
            AddInclude(b => b.OrderType);
            AddInclude(b => b.OU);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.Supplier);
            AddInclude(b => b.SupplierSite);
            AddInclude(b => b.PhyWarehouse);
        }
    }
}
