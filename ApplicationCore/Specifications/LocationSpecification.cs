using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class LocationSpecification:BaseSpecification<Location>
    {
        public LocationSpecification(int? id, string locationCode,int? wareHouseId,int? areaId,int? type,int? status,int? inStock)
            : base(b => (!id.HasValue || b.Id == id) &&
                      (locationCode==null || b.SysCode == locationCode)&&
                       (!wareHouseId.HasValue || b.Warehouse.Id == wareHouseId)&&
                       (!areaId.HasValue || b.ReservoirArea.Id == areaId) &&
                       (!type.HasValue || b.Type == type) &&
                       (!status.HasValue || b.Status == status) &&
                       (!inStock.HasValue || b.InStock == inStock))
        {
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.ReservoirArea);
        }
    }
}
