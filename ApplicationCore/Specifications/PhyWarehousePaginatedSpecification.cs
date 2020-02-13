using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class PhyWarehousePaginatedSpecification : BaseSpecification<PhyWarehouse>
    {
        public PhyWarehousePaginatedSpecification(int skip, int take, int? id,string phyName)
            :base(b=>(!id.HasValue|| b.Id==id)&&
                     phyName==null||b.PhyName.Contains(phyName))
        {
           
        }
    }
}
