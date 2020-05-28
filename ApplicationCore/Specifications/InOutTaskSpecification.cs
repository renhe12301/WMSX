using System;
using System.Collections.Generic;
using ApplicationCore.Entities.TaskManager;

namespace ApplicationCore.Specifications
{
    public class InOutTaskSpecification:BaseSpecification<InOutTask>
    {
        public InOutTaskSpecification(int? id,string trayCode,List<int> status,List<int> steps,List<int> types,
            int? isRead,int? ouId,int? wareHouseId, int? areaId,int? pyId,
            string sCreateTime, string eCreateTime,
            string sFinishTime,string eFinishTime)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (trayCode==null||b.TrayCode==trayCode)&&
                     (!ouId.HasValue || b.OUId == ouId) &&
                     (!wareHouseId.HasValue || b.WarehouseId == wareHouseId) &&
                     (!areaId.HasValue || b.ReservoirAreaId == areaId) &&
                     (status==null||status.Contains(b.Status))&&
                     (!pyId.HasValue || b.PhyWarehouseId == pyId) &&
                     (steps == null || steps.Contains(b.Step)) &&
                     (types == null || types.Contains(b.Type)) &&
                     (!isRead.HasValue || b.IsRead == isRead) &&
                     (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                     (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime))&&
                     (sFinishTime==null||b.FinishTime>=DateTime.Parse(sFinishTime))&&
                     (eFinishTime==null||b.FinishTime<=DateTime.Parse(eFinishTime)))
        {

            AddInclude(b=>b.ReservoirArea);
            AddInclude(b=>b.Warehouse);
            AddInclude(b=>b.OU);
            AddInclude(b => b.SubOrder);
            AddInclude(b => b.SubOrderRow);
            AddInclude(b => b.PhyWarehouse);
            AddInclude(b=>b.WarehouseTray);
        }
    }
}
