using System;
using System.Collections.Generic;
using ApplicationCore.Entities.TaskManager;

namespace ApplicationCore.Specifications
{
    public class InOutTaskPaginatedSpecification:BaseSpecification<InOutTask>
    {
        public InOutTaskPaginatedSpecification(int skip,int take,int? id,string trayCode,string materialCode,int? subOrderId,int? subOrderRowId,
            List<int> status,List<int> steps,List<int> types, int? isRead,int? ouId, int? wareHouseId, int? areaId,int? pyId,
            string sCreateTime, string eCreateTime,
            string sFinishTime,string eFinishTime)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (trayCode==null||b.TrayCode==trayCode)&&
                     (materialCode==null||b.MaterialCode==materialCode)&&
                     (!subOrderId.HasValue || b.SubOrderId == subOrderId) &&
                     (!subOrderRowId.HasValue || b.SubOrderRowId == subOrderRowId) &&
                     (!ouId.HasValue || b.OUId == ouId) &&
                     (!ouId.HasValue || b.OUId == ouId) &&
                     (!wareHouseId.HasValue || b.WarehouseId == wareHouseId) &&
                     (!areaId.HasValue || b.ReservoirAreaId == areaId) &&
                     (!pyId.HasValue || b.PhyWarehouseId == pyId) &&
                     (!isRead.HasValue || b.IsRead == isRead) &&
                     (status==null||status.Contains(b.Status))&&
                     (steps == null || steps.Contains(b.Step)) &&
                     (types == null || types.Contains(b.Type)) &&
                     (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                     (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime))&&
                     (sFinishTime==null||b.FinishTime>=DateTime.Parse(sFinishTime))&&
                     (eFinishTime==null||b.FinishTime<=DateTime.Parse(eFinishTime)))
        {
            ApplyPaging(skip, take);
            AddInclude(b=>b.ReservoirArea);
            AddInclude(b=>b.Warehouse);
            AddInclude(b=>b.OU);
            AddInclude(b => b.SubOrder);
            AddInclude(b => b.SubOrderRow);
            AddInclude(b => b.PhyWarehouse);
            AddInclude(b=>b.WarehouseTray);
            AddInclude(b=>b.MaterialDic);
        }
    }
}
