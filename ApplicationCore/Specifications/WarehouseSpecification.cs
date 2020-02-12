using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class WarehouseSpecification:BaseSpecification<Warehouse>
    {
        public WarehouseSpecification(int? id,int? ouId,string whName)
            :base(b=>(!id.HasValue|| b.Id==id)&&
                  (!ouId.HasValue || b.OUId == ouId)&&
                  whName==null||b.WhName.Contains(whName))
        {
            AddInclude(b => b.OU);
        }
    }
}
