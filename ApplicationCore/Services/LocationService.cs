using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class LocationService:ILocationService
    {
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        public LocationService(IAsyncRepository<Location> locationRepository,
                               IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                               IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository)
        {
            this._locationRepository = locationRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
        }

        public async Task AddLocation(Location location)
        {
            Guard.Against.Null(location, nameof(location));
            Guard.Against.NullOrEmpty(location.SysCode,nameof(location.SysCode));
            await this._locationRepository.AddAsync(location);
        }

        public async Task BuildLocation(int orgId,int row, int rank, int col)
        {
            Guard.Against.Zero(orgId, nameof(orgId));
            Guard.Against.Zero(row, nameof(row));
            Guard.Against.Zero(rank, nameof(rank));
            Guard.Against.Zero(col, nameof(col));

            List<Location> addLocations=new List<Location>();
            DateTime now=DateTime.Now;
            for (int i = 1; i <=row; i++)
            {
                for (int j = 1; j <= rank; j++)
                {
                    for (int k = 1; k <= col; k++)
                    {
                        string code = orgId.ToString()+"-"+i.ToString().PadLeft(3, '0') +"-"+
                                      j.ToString().PadLeft(3, '0') +"-"+
                                      k.ToString().PadLeft(3, '0');
                        string locationCode = code;
                        Location location = new Location
                        {
                            SysCode = locationCode,
                            UserCode = locationCode,
                            CreateTime = now,
                            Floor = i,
                            Item = j,
                            Col = k
                        };
                        addLocations.Add(location);
                    }
                }
            }
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    addLocations.ForEach(l =>
                    {
                        this._locationRepository.AddAsync(l);
                    });
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
           
        }

        public async Task Clear(List<int> ids)
        {
            Guard.Against.NullOrEmpty(ids, nameof(ids));
            var locations = await this._locationRepository.ListAllAsync();
            Guard.Against.NullOrEmpty(locations, nameof(locations));
            var wareHouseTrays = await this._warehouseTrayRepository.ListAllAsync();
            var wareHouseMaterials = await this._warehouseMaterialRepository.ListAllAsync();
            List<WarehouseTray> delWareHouseTrays=new List<WarehouseTray>();
            List<WarehouseMaterial> delWareHouseMaterials=new List<WarehouseMaterial>();
            List<Location> updLocations=new List<Location>();
            locations.ForEach(l =>
            {
                if (ids.Contains(l.Id))
                {
                    if(l.IsTask==Convert.ToInt32(LOCATION_TASK.有任务))
                        throw new Exception(string.Format("货位[{0}]当前有任务,无法禁用！",l.SysCode));
                    delWareHouseTrays.AddRange(wareHouseTrays.FindAll(wht=>wht.LocationId==l.Id));
                    delWareHouseMaterials.AddRange(wareHouseMaterials.FindAll(whm=>whm.LocationId==l.Id));
                    l.InStock = Convert.ToInt32(LOCATION_INSTOCK.无货);
                    updLocations.Add(l);
                }
            });
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    this._warehouseTrayRepository.Delete(delWareHouseTrays);
                    this._warehouseMaterialRepository.Delete(delWareHouseMaterials);
                    this._locationRepository.Update(updLocations);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task Disable(List<int> ids)
        {
            Guard.Against.NullOrEmpty(ids, nameof(ids));
            var locations = await this._locationRepository.ListAllAsync();
            Guard.Against.NullOrEmpty(locations, nameof(locations));
            
            List<Location> updLocations=new List<Location>();
            locations.ForEach(l =>
            {
                if (ids.Contains(l.Id))
                {
                    if(l.IsTask==Convert.ToInt32(LOCATION_TASK.有任务))
                        throw new Exception(string.Format("货位[{0}]当前有任务,无法禁用！",l.SysCode));
                    l.Status = Convert.ToInt32(LOCATION_STATUS.禁用);
                    updLocations.Add(l);
                }
            });
            await this._locationRepository.UpdateAsync(updLocations);
        }

        public async Task Enable(List<int> ids)
        {
            Guard.Against.NullOrEmpty(ids, nameof(ids));
            var locations = await this._locationRepository.ListAllAsync();
            Guard.Against.NullOrEmpty(locations, nameof(locations));

            List<Location> updLocations = new List<Location>();
            locations.ForEach(l =>
            {
                if (ids.Contains(l.Id))
                {
                    if (l.IsTask == Convert.ToInt32(LOCATION_TASK.有任务))
                        throw new Exception(string.Format("货位[{0}]当前有任务,无法启用！", l.SysCode));
                    l.Status = Convert.ToInt32(LOCATION_STATUS.正常);
                    updLocations.Add(l);
                }
            });
            await this._locationRepository.UpdateAsync(updLocations);
        }

        public async Task UpdateLocation(int id, string sysCode, string userCode)
        {
            Guard.Against.Zero(id, nameof(id));
            var locationSpec = new LocationSpecification(id, null, null, null, null,null,
                null, null, null, null, null,null,null);
            var sysCodelocationSpec = new LocationSpecification(null, sysCode, null, null,null, null,
                null, null, null, null, null,null,null);
            var userCodelocationSpec = new LocationSpecification(null, null, userCode, null,null, null,
                null, null, null, null, null,null,null);
            var locations = await this._locationRepository.ListAsync(locationSpec);
            Guard.Against.NullOrEmpty(locations, nameof(locations));
            var location = locations[0];
            if (!string.IsNullOrEmpty(sysCode))
            {
                var sysCodelocations = await this._locationRepository.ListAsync(sysCodelocationSpec);
                if (sysCodelocations.Count > 0)
                {
                    if(sysCodelocations[0].Id!=id)
                        throw new Exception(string.Format("货位系统编号[{0}],已经存在！",sysCode));
                }
                location.SysCode = sysCode;
            }
            if (!string.IsNullOrEmpty(userCode))
            {
                var userCodelocations = await this._locationRepository.ListAsync(userCodelocationSpec);
                if (userCodelocations.Count > 0)
                {
                    if(userCodelocations[0].Id!=id)
                        throw new Exception(string.Format("货位用户编号[{0}],已经存在！",userCode));
                }
                location.UserCode = userCode;
            }
            await this._locationRepository.UpdateAsync(location);
        }
    }
}
