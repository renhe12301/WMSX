﻿using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;
namespace ApplicationCore.Specifications
{
    public class SubOrderRowPaginatedSpecification : BaseSpecification<SubOrderRow>
    {
        public SubOrderRowPaginatedSpecification(int skip, int take, List<int> ids, int? subOrderId, int? orderRowId, int? sourceId,
            List<int> orderTypeIds, int? ouId, int? warehouseId, int? reservoirAreaId, string businessType, string ownerType, int? pyId, int? supplierId, string supplierName, int? supplierSiteId,
            string supplierSiteName, List<int> status, string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime)
            : base(b => (ids == null || ids.Contains(b.Id)) &&
                   (!subOrderId.HasValue || b.SubOrderId == subOrderId) &&
                   (!orderRowId.HasValue || b.OrderRowId == orderRowId) &&
                   (!sourceId.HasValue || b.SourceId == sourceId) &&
                   (orderTypeIds == null || orderTypeIds.Contains(b.SubOrder.OrderTypeId)) &&
                   (!ouId.HasValue || b.SubOrder.OUId == ouId) &&
                   (!warehouseId.HasValue || b.SubOrder.WarehouseId == warehouseId) &&
                   (!reservoirAreaId.HasValue || b.ReservoirAreaId == reservoirAreaId) &&
                   (businessType == null || b.SubOrder.BusinessTypeCode == businessType) &&
                   (ownerType==null || b.OwnerType == ownerType) &&
                   (!pyId.HasValue || b.SubOrder.PhyWarehouseId == pyId) &&
                   (!supplierId.HasValue || b.SubOrder.SupplierId == supplierId) &&
                   (supplierName == null || b.SubOrder.Supplier.SupplierName.Contains(supplierName)) &&
                   (!supplierSiteId.HasValue || b.SubOrder.SupplierSiteId == supplierSiteId) &&
                   (supplierSiteName == null || b.SubOrder.SupplierSite.SiteName.Contains(supplierSiteName)) &&
                   (status == null || status.Contains(b.Status.GetValueOrDefault())) &&
                   (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                   (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)) &&
                   (sFinishTime == null || b.FinishTime >= DateTime.Parse(sFinishTime)) &&
                   (eFinishTime == null || b.FinishTime <= DateTime.Parse(eFinishTime)))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.SubOrder);
            AddInclude(b => b.MaterialDic);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.OrderRow);
        }
    }
}
