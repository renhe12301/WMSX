using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class ReservoirAreaSpecification:BaseSpecification<ReservoirArea>
    {
        public ReservoirAreaSpecification(int? id,string areaCode, int? ouId,int? whId,int? type,string areaName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (areaCode==null||b.AreaCode==areaCode)&&
                      (!ouId.HasValue || b.OUId == ouId) &&
                      (!whId.HasValue||b.WarehouseId==whId)&&
                      (!type.HasValue || b.Type == type) &&
                      (areaName==null||b.AreaName.Contains(areaName)))
        {
            AddInclude(b => b.Warehouse);
            AddInclude(b=>b.OU);
            AddInclude(b=>b.PhyWarehouse);
        }
    }
}
