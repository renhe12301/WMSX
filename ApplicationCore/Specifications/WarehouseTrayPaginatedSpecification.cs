using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Specifications
{
    public class WarehouseTrayPaginatedSpecification : BaseSpecification<WarehouseTray>
    {
        public WarehouseTrayPaginatedSpecification(int skip, int take, int? id, string trayCode,
            List<int> rangeMaterialCount,
            int? orderId, int? orderRowId, int? carrier,
            List<int> traySteps, int? locationId, int? ouId, int? wareHouseId, int? areaId, int? pyId)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (trayCode == null || b.TrayCode == trayCode) &&
                        (rangeMaterialCount == null || (b.MaterialCount >= rangeMaterialCount[0] &&
                                                        b.MaterialCount <= rangeMaterialCount[1])) &&
                        (!orderRowId.HasValue || b.SubOrderRowId == orderRowId) &&
                        (!orderId.HasValue || b.SubOrderId == orderId) &&
                        (carrier == null || b.Carrier == carrier) &&
                        (traySteps == null || traySteps.Contains(b.TrayStep.Value)) &&
                        (!locationId.HasValue || b.LocationId == locationId) &&
                        (!ouId.HasValue || b.OUId == ouId) &&
                        (!wareHouseId.HasValue || b.WarehouseId == wareHouseId) &&
                        (!areaId.HasValue || b.ReservoirAreaId == areaId) &&
                        (!pyId.HasValue || b.PhyWarehouseId == pyId))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Location);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.SubOrder);
            AddInclude(b => b.SubOrderRow);
            AddInclude(b => b.PhyWarehouse);
        }
    }
}
