using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Specifications
{
    public class WarehouseTraySpecification: BaseSpecification<WarehouseTray>
    {
        public WarehouseTraySpecification(int? id,string trayCode,List<double> rangeMaterialCount,
            int? subOrderId,int? subOrderRowId, int? carrier,List<int> trayStatus,int? locationId,
             int? ouId, int? wareHouseId,int? areaId,int? pyId)
            :base(b =>(!id.HasValue || b.Id == id) &&
                      (trayCode == null || b.TrayCode == trayCode) &&
                      (rangeMaterialCount==null|| (b.MaterialCount >= rangeMaterialCount[0]&& b.MaterialCount <= rangeMaterialCount[1])) &&
                      (!subOrderId.HasValue || b.SubOrderId == subOrderId) &&
                      (!subOrderRowId.HasValue || b.SubOrderRowId == subOrderRowId) &&
                      (carrier == null || b.Carrier == carrier)&&
                      (trayStatus == null || trayStatus.Contains(b.TrayStep.Value)) &&
                      (!locationId.HasValue||b.LocationId==locationId)&&
                      (!ouId.HasValue || b.OUId == ouId) &&
                      (!wareHouseId.HasValue||b.WarehouseId==wareHouseId)&&
                      (!areaId.HasValue||b.ReservoirAreaId==areaId)&&
                      (!pyId.HasValue||b.PhyWarehouseId==pyId))
        {
            AddInclude(b => b.Location);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.SubOrder);
            AddInclude(b => b.SubOrderRow);
            AddInclude(b => b.PhyWarehouse);
            AddInclude(b => b.OU);
        }
    }
}
