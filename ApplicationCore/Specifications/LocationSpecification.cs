using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class LocationSpecification:BaseSpecification<Location>
    {
        public LocationSpecification(int? id, string locationCode,int? orgId,int? ouId,
            int? wareHouseId,int? areaId,List<int> types,List<int> status,List<int> inStocks)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (locationCode==null || b.SysCode == locationCode)&&
                        (!orgId.HasValue || b.Organization.Id == orgId) &&
                        (!ouId.HasValue || b.OU.Id == ouId) &&
                       (!wareHouseId.HasValue || b.Warehouse.Id == wareHouseId)&&
                       (!areaId.HasValue || b.ReservoirArea.Id == areaId) &&
                       (types==null || types.Contains(b.Type)) &&
                       (status==null || status.Contains(b.Status)) &&
                       (inStocks==null || inStocks.Contains(b.InStock)))
        {
            AddInclude(b => b.Organization);
            AddInclude(b => b.OU);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.ReservoirArea);
        }
    }
}
