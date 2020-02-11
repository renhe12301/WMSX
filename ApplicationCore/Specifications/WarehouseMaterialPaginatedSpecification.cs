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
            string materialName,string materialSpec,string trayCode, int? trayDicId,int? orderId,int? orderRowId, 
            int? carrier, List<int> traySteps, int? locationId, int? ouId, int? wareHouseId, int? areaId)
            : base(b =>(!id.HasValue || b.Id == id) &&
                       (materialCode == null || b.MaterialDic.MaterialCode == materialCode) &&
                       (!materialDicId.HasValue || b.MaterialDicId == materialDicId) &&
                       (materialName == null || b.MaterialDic.MaterialName.Contains(materialName)) &&
                       (materialSpec == null || b.MaterialDic.Spec.Contains(materialSpec)) &&
                       (trayCode == null || b.WarehouseTray.TrayCode == trayCode) &&
                       (!trayDicId.HasValue || b.WarehouseTray.Id == trayDicId) &&
                       (!orderId.HasValue || b.OrderId == orderId) &&
                       (!carrier.HasValue || b.Carrier == carrier) &&
                       (!orderRowId.HasValue || b.OrderRowId == orderRowId) &&
                       (traySteps == null || traySteps.Contains(b.WarehouseTray.TrayStep.Value)) &&
                       (!locationId.HasValue || b.LocationId == locationId) &&
                       (!ouId.HasValue || b.OUId == ouId) &&
                       (!wareHouseId.HasValue || b.WarehouseId == wareHouseId) &&
                       (!areaId.HasValue || b.ReservoirAreaId == areaId))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.MaterialDic);
            AddInclude(b => b.Location);
            AddInclude(b => b.WarehouseTray);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.Order);
            AddInclude(b => b.OrderRow);
            AddInclude(b=>b.OU);
        }
    }
}
