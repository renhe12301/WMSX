using System;
using System.Collections.Generic;
using ApplicationCore.Entities.TaskManager;

namespace ApplicationCore.Specifications
{
    public class InOutTaskPaginatedSpecification:BaseSpecification<InOutTask>
    {
        public InOutTaskPaginatedSpecification(int skip,int take,int? id,
            List<int> status,List<int> steps, int? orgId, int? ouId, int? wareHouseId, int? areaId,
            string sCreateTime, string eCreateTime,
            string sFinishTime,string eFinishTime)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (!orgId.HasValue || b.OrganizationId == orgId) &&
                     (!ouId.HasValue || b.OUId == ouId) &&
                     (!wareHouseId.HasValue || b.WarehouseId == wareHouseId) &&
                     (!areaId.HasValue || b.ReservoirAreaId == areaId) &&
                     (status==null||status.Contains(b.Status))&&
                     (steps == null || steps.Contains(b.Step)) &&
                     (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                     (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime))&&
                     (sFinishTime==null||b.FinishTime>=DateTime.Parse(sFinishTime))&&
                     (eFinishTime==null||b.FinishTime<=DateTime.Parse(eFinishTime)))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Order);
            AddInclude(b => b.OrderRow);
            AddInclude(b => b.WarehouseTray);
        }
    }
}
