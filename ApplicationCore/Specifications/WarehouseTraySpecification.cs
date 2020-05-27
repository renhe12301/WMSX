﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Specifications
{
    public class WarehouseTraySpecification: BaseSpecification<WarehouseTray>
    {
        public WarehouseTraySpecification(int? id,string trayCode,List<int> rangeMaterialCount,
            int? orderId,int? orderRowId, int? carrier,List<int> trayStatus,int? locationId,
             int? ouId, int? wareHouseId,int? areaId,int? pyId)
            :base(b =>(!id.HasValue || b.Id == id) &&
                      (trayCode == null || b.TrayCode == trayCode) &&
                      (rangeMaterialCount==null|| (b.MaterialCount >= rangeMaterialCount[0]&& b.MaterialCount <= rangeMaterialCount[1])) &&
                      (!orderId.HasValue || b.SubOrderId == orderId) &&
                      (!orderRowId.HasValue || b.SubOrderRowId == orderRowId) &&
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
        }
    }
}
