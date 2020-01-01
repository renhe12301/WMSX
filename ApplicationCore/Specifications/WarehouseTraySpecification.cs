﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Specifications
{
    public class WarehouseTraySpecification: BaseSpecification<WarehouseTray>
    {
        public WarehouseTraySpecification(int? id,string trayCode,List<int> rangeMaterialCount,int? trayDicId,
            int? orderId,int? orderRowId, int? carrier,List<int> trayStatus,int? locationId,
            int? wareHouseId,int? areaId)
            :base(b =>(!id.HasValue || b.Id == id) &&
                      (trayCode == null || b.Code == trayCode) &&
                      (rangeMaterialCount==null|| (b.MaterialCount >= rangeMaterialCount[0]&& b.MaterialCount <= rangeMaterialCount[0])) &&
                      (!trayDicId.HasValue || b.TrayDicId == trayDicId)&&
                      (!orderId.HasValue || b.OrderId == orderId) &&
                      (!orderRowId.HasValue || b.OrderRowId == orderRowId) &&
                      (carrier == null || b.Carrier == carrier)&&
                      (trayStatus == null || trayStatus.Contains(b.TrayStep.Value)) &&
                      (!locationId.HasValue||b.LocationId==locationId)&&
                      (!wareHouseId.HasValue||b.WarehouseId==wareHouseId)&&
                      (!areaId.HasValue||b.ReservoirAreaId==areaId))
        {
            AddInclude(b => b.TrayDic);
            AddInclude(b => b.Location);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.Order);
            AddInclude(b => b.OrderRow);
        }
    }
}
