using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class ReservoirAreaSpecification:BaseSpecification<ReservoirArea>
    {
        public ReservoirAreaSpecification(int? id,string areaCode, int? ouId,int? whId,string ownerType,string areaName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (areaCode==null||b.AreaCode==areaCode)&&
                      (!ouId.HasValue || b.OUId == ouId) &&
                      (!whId.HasValue||b.WarehouseId==whId)&&
                      (areaName==null||b.AreaName.Contains(areaName))&&
                      (ownerType == null || b.OwnerType==ownerType)
                  )
        {
            AddInclude(b => b.Warehouse);
            // AddInclude(b=>b.OU);
            AddInclude(b=>b.PhyWarehouse);
        }
    }
}
