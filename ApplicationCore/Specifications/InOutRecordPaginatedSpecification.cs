using System;
using System.Collections.Generic;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Specifications
{
    public class InOutRecordPaginatedSpecification : BaseSpecification<InOutRecord>
    {
        public InOutRecordPaginatedSpecification(int skip,int take,string trayCode,string materialName,int? type,int? ouId,int? wareHouseId, 
                                                int? areaId,int? pyId,int? orderId,int? orderRowId,int? orderRowBatchId,
                                                List<int> status,int? isRead,string sCreateTime,string eCreateTime)
            : base(b => (!type.HasValue || b.Type == type) &&
                                           (!ouId.HasValue || b.OUId == ouId)&&
                                           (trayCode==null||b.TrayCode==trayCode)&&
                                           (materialName==null||b.MaterialDic.MaterialName.Contains(materialName))&&
                                           (!wareHouseId.HasValue || b.WarehouseId == wareHouseId)&&
                                           (!areaId.HasValue || b.ReservoirAreaId == areaId)&&
                                           (!orderId.HasValue || b.OrderId == orderId)&&
                                           (!orderRowId.HasValue || b.OrderRowId == orderRowId)&&
                                           (!orderRowBatchId.HasValue || b.OrderRowBatchId == orderRowBatchId)&&
                                           (!pyId.HasValue || b.PhyWarehouseId == pyId)&&
                                           (!isRead.HasValue || b.IsRead == isRead)&&
                                           (status==null||status.Contains(b.Status))&&
                                           (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                                           (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime)))
        {
            ApplyPaging(skip, take);
            AddInclude(b=>b.MaterialDic);
            AddInclude(b=>b.OU);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b=>b.Order);
            AddInclude(b=>b.OrderRowBatch);
            AddInclude(b=>b.PhyWarehouse);
            
        }
    }
}
