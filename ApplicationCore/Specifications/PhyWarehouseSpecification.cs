using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class PhyWarehouseSpecification:BaseSpecification<PhyWarehouse>
    {
        public PhyWarehouseSpecification(int? id,string phyName)
            :base(b=>(!id.HasValue|| b.Id==id)&&
                     phyName==null||b.PhyName.Contains(phyName))
        {
           
        }
    }
}
