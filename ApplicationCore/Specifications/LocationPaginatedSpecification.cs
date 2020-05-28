using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class LocationPaginatedSpecification:BaseSpecification<Location>
    {
        public LocationPaginatedSpecification(int skip,int take,int? id, string sysCode,string userCode,List<int> types,
            int? phyId,int? ouId, int? wareHouseId,int? areaId,List<int> status,List<int> inStocks,List<int> isTasks,
            List<int> floors,List<int> items,List<int> cols)
            : base(b => (!id.HasValue || b.Id == id) &&
                        (sysCode==null || b.SysCode.Contains(sysCode))&&
                        (userCode==null || b.UserCode.Contains(userCode))&&
                        (types==null || types.Contains(b.Type.GetValueOrDefault())) &&
                        (!phyId.HasValue || b.PhyWarehouseId == phyId) &&
                        (!ouId.HasValue || b.OUId== ouId) &&
                       (!wareHouseId.HasValue || b.WarehouseId == wareHouseId)&&
                       (!areaId.HasValue || b.ReservoirAreaId == areaId) &&
                        (status==null || status.Contains(b.Status)) &&
                        (inStocks==null || inStocks.Contains(b.InStock))&&
                        (isTasks==null || isTasks.Contains(b.IsTask))&&
                        (floors==null || floors.Contains(b.Floor.Value)) &&
                        (items==null || items.Contains(b.Item.Value))&&
                        (cols==null || cols.Contains(b.Col.Value)))
        {
            ApplyPaging(skip,take);
            AddInclude(b => b.OU);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.ReservoirArea);
            AddInclude(b=>b.PhyWarehouse);
        }
    }
}
