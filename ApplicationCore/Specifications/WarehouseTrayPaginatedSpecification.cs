using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Specifications
{
    public class WarehouseTrayPaginatedSpecification : BaseSpecification<WarehouseTray>
    {
        public WarehouseTrayPaginatedSpecification(int skip,int take,int? id,string trayCode, List<int> rangeMaterialCount,
             int? orderId,int? orderRowId, int? carrier,
            List<int> traySteps, int? locationId, int? ouId, int? wareHouseId,int? areaId)
            :base(b =>(!id.HasValue || b.Id == id) &&
                      (trayCode == null || b.TrayCode == trayCode) &&
                      (rangeMaterialCount == null || (b.MaterialCount >= rangeMaterialCount[0] && b.MaterialCount <= rangeMaterialCount[0])) &&
                      (!orderRowId.HasValue || b.OrderRowId == orderRowId) &&
                      (!orderId.HasValue || b.OrderId == orderId) &&
                      (carrier == null || b.Carrier == carrier)&&
                      (traySteps == null || traySteps.Contains(b.TrayStep.Value)) &&
                      (!locationId.HasValue||b.LocationId==locationId)&&
                      (!ouId.HasValue || b.OUId == ouId) &&
                      (!wareHouseId.HasValue||b.WarehouseId==wareHouseId)&&
                      (!areaId.HasValue||b.ReservoirAreaId==areaId))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Location);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.Order);
            AddInclude(b => b.OrderRow);

        }
    }
}
