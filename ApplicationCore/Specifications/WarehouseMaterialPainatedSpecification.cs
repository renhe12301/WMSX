using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Specifications
{
    public class WarehouseMaterialPainatedSpecification: BaseSpecification<WarehouseMaterial>
    {
        public WarehouseMaterialPainatedSpecification(int skip,int take,int? id, string materialCode, int? materialDicId,
            string trayCode, int? trayDicId,int? orderId,int? orderRowId, int? carrier, List<int> traySteps, int? locationId,
            int? wareHouseId, int? areaId)
            : base(b => (!id.HasValue || b.Id == id) &&
                       (materialCode == null || b.MaterialDic.MaterialCode == materialCode) &&
                       (!materialDicId.HasValue || b.MaterialDicId == materialDicId) &&
                       (trayCode == null || b.WarehouseTray.Code == trayCode) &&
                       (!trayDicId.HasValue || b.WarehouseTray.Id == trayDicId) &&
                       (!orderId.HasValue || b.OrderId == orderId) &&
                       (!carrier.HasValue || b.Carrier == carrier) &&
                       (!orderRowId.HasValue || b.OrderRowId == orderRowId) &&
                       (traySteps == null || traySteps.Contains(b.WarehouseTray.TrayStep.Value)) &&
                       (!locationId.HasValue || b.LocationId == locationId) &&
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
        }
    }
}
