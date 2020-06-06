using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Specifications
{
    public class WarehouseMaterialPaginatedSpecification: BaseSpecification<WarehouseMaterial>
    {
        public WarehouseMaterialPaginatedSpecification(int skip,int take,int? id, string materialCode, int? materialDicId,
            string materialName,string materialSpec,string trayCode, int? warehouseTrayId,int? subOrderId,int? subOrderRowId, 
            int? carrier, List<int> traySteps, int? locationId, int? ouId, int? wareHouseId, int? areaId,int? supplierId,
            int? supplierSiteId,int? pyId,string sCreateTime,string eCreateTime)
            : base(b =>(!id.HasValue || b.Id == id) &&
                       (materialCode == null || b.MaterialDic.MaterialCode == materialCode) &&
                       (!materialDicId.HasValue || b.MaterialDicId == materialDicId) &&
                       (materialName == null || b.MaterialDic.MaterialName.Contains(materialName)) &&
                       (materialSpec == null || b.MaterialDic.Spec.Contains(materialSpec)) &&
                       (trayCode == null || b.WarehouseTray.TrayCode == trayCode) &&
                       (!warehouseTrayId.HasValue || b.WarehouseTray.Id == warehouseTrayId) &&
                       (!subOrderId.HasValue || b.SubOrderId == subOrderId) &&
                       (!carrier.HasValue || b.Carrier == carrier) &&
                       (!subOrderRowId.HasValue || b.SubOrderRowId == subOrderRowId) &&
                       (traySteps == null || traySteps.Contains(b.WarehouseTray.TrayStep.Value)) &&
                       (!locationId.HasValue || b.LocationId == locationId) &&
                       (!ouId.HasValue || b.OUId == ouId) &&
                       (!wareHouseId.HasValue || b.WarehouseId == wareHouseId) &&
                       (!areaId.HasValue || b.ReservoirAreaId == areaId)&&
                       (!supplierId.HasValue||b.SupplierId==supplierId)&&
                       (!supplierSiteId.HasValue||b.SupplierSiteId==supplierSiteId)&&
                       (!pyId.HasValue||b.PhyWarehouseId==pyId)&&
                       (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                       (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime)))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.MaterialDic);
            AddInclude(b => b.Location);
            AddInclude(b => b.WarehouseTray);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.SubOrder);
            AddInclude(b => b.SubOrderRow);
            AddInclude(b=>b.OU);
            AddInclude(b=>b.Supplier);
            AddInclude(b=>b.SupplierSite);
            AddInclude(b=>b.PhyWarehouse);
        }
    }
}
