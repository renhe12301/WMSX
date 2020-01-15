using System;
using System.Threading.Tasks;
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
            await this._locationRepository.AddAsync(location);
        }

        public async Task BuildLocation(int wareHouseId,int row, int rank, int col)
        {
            Guard.Against.Zero(wareHouseId, nameof(wareHouseId));
            Guard.Against.Zero(row, nameof(row));
            Guard.Against.Zero(rank, nameof(rank));
            Guard.Against.Zero(col, nameof(col));

            this._locationRepository.TransactionScope(async() =>
            {

                for (int i = 1; i <=row; i++)
                {
                    for (int j = 1; j <= rank; j++)
                    {
                        for (int k = 1; k <= col; k++)
                        {
                            string code = wareHouseId.ToString()+"-"+i.ToString().PadRight(3, '0') +"-"+
                                          j.ToString().PadRight(3, '0') +"-"+
                                          k.ToString().PadRight(3, '0');
                            string locationCode = code;
                            Location location = new Location
                            {
                                SysCode = locationCode,
                                CreateTime = DateTime.Now

                            };
                            await this._locationRepository.AddAsync(location);
                           
                        }
                    }
                }

            });
        }

        public async Task Clear(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            this._warehouseTrayRepository.TransactionScope(async() =>
            {
                var wareHouseTraySpec = new WarehouseTraySpecification(null, null,
                    null,null, null,null, null, null, id,null,null, null, null);
                var wareHouseTrays = await this._warehouseTrayRepository.ListAsync(wareHouseTraySpec);
                if (wareHouseTrays.Count > 0)
                    await this._warehouseTrayRepository.DeleteAsync(wareHouseTrays);

                var wareHouseMaterialSpec = new WarehouseMaterialSpecification(null,null,null,null,
                    null, null, null, null, null, id,null,null, null, null);
                var wareHouseMaterials = await this._warehouseMaterialRepository.ListAsync(wareHouseMaterialSpec);
                if (wareHouseMaterials.Count > 0)
                    await this._warehouseMaterialRepository.DeleteAsync(wareHouseMaterials);
            });
        }

        public async Task Disable(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            var locationSpec = new LocationSpecification(id, null,null,null,null,null,null,null,null);
            var locations = await this._locationRepository.ListAsync(locationSpec);
            Guard.Against.NullOrEmpty(locations, nameof(locations));
            var location = locations[0];
            location.Status = Convert.ToInt32(LOCATION_STATUS.禁用);
            await this._locationRepository.UpdateAsync(location);
        }

        public async Task Enable(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            var locationSpec = new LocationSpecification(id, null,null,null,null,null,null,null,null);
            var locations = await this._locationRepository.ListAsync(locationSpec);
            Guard.Against.NullOrEmpty(locations, nameof(locations));
            var location = locations[0];
            location.Status = Convert.ToInt32(LOCATION_STATUS.正常);
            await this._locationRepository.UpdateAsync(location);
        }

        public async Task Lock(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            var locationSpec = new LocationSpecification(id, null,null,null,null,null,null,null,null);
            var locations = await this._locationRepository.ListAsync(locationSpec);
            Guard.Against.NullOrEmpty(locations, nameof(locations));
            var location = locations[0];
            location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
            await this._locationRepository.UpdateAsync(location);
        }

        public async Task UnLock(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            var locationSpec = new LocationSpecification(id,null,null,null,null,null,null,null,null);
            var locations = await this._locationRepository.ListAsync(locationSpec);
            Guard.Against.NullOrEmpty(locations, nameof(locations));
            var location = locations[0];
            location.Status = Convert.ToInt32(LOCATION_STATUS.正常);
            await this._locationRepository.UpdateAsync(location);
        }

        public async Task UpdateLocation(int id, string userCode)
        {
            Guard.Against.Zero(id, nameof(id));
            Guard.Against.NullOrEmpty(userCode, nameof(userCode));
            var locationSpec = new LocationSpecification(id,null,null,null,null,null,null,null,null);
            var locations = await this._locationRepository.ListAsync(locationSpec);
            Guard.Against.NullOrEmpty(locations, nameof(locations));
            var location = locations[0];
            location.UserCode = userCode;
            await this._locationRepository.UpdateAsync(location);
        }
    }
}
