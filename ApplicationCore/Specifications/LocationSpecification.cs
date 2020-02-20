using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class LocationSpecification:BaseSpecification<Location>
    {
        public LocationSpecification(int? id, string sysCode,string userCode,int? type,int? phyId,int? ouId,
            int? wareHouseId,int? areaId,List<int> status,List<int> inStocks,List<int> isTasks,
            List<int> floors,List<int> items,List<int> cols)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (sysCode==null || b.SysCode.Contains(sysCode))&&
                        (userCode==null || b.UserCode.Contains(userCode))&&
                        (!type.HasValue || b.Type==type) &&
                        (!phyId.HasValue || b.PhyWarehouseId == phyId) &&
                        (!ouId.HasValue || b.OUId == ouId) &&
                        (!wareHouseId.HasValue || b.WarehouseId == wareHouseId)&&
                        (!areaId.HasValue || b.ReservoirAreaId == areaId) &&
                        (status==null || status.Contains(b.Status)) &&
                        (inStocks==null || inStocks.Contains(b.InStock))&&
                        (isTasks==null || isTasks.Contains(b.IsTask))&&
                        (floors==null || floors.Contains(b.Floor.Value)) &&
                        (items==null || items.Contains(b.Item.Value))&&
                        (cols==null || cols.Contains(b.Col.Value)))
        {
            ApplyOrderBy(b=>b.Floor&b.Item&b.Col);
            AddInclude(b => b.OU);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b=>b.PhyWarehouse);
        }
    }
}
