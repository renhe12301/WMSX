using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class WarehouseSpecification:BaseSpecification<Warehouse>
    {
        public WarehouseSpecification(int? id,int? ouId,string whName,string whCode)
            :base(b=>(!id.HasValue|| b.Id==id)&&
                  (!ouId.HasValue || b.OUId == ouId)&&
                  (whName==null||b.WhName.Contains(whName))&&
                  (whCode==null||b.WhCode.Contains(whCode)))
        {
            AddInclude(b => b.OU);
            AddInclude(b=>b.PhyWarehouse);
        }
    }
}
