using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class ReservoirAreaService:IReservoirAreaService
    {
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<MaterialDicTypeArea> _areaMaterialRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        public ReservoirAreaService(IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                                    IAsyncRepository<Location> locationRepository,
                                    IAsyncRepository<MaterialDicTypeArea> areaMaterialRepository)
        {
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._locationRepository = locationRepository;
            this._areaMaterialRepository = areaMaterialRepository;
        }

        public async Task AddArea(ReservoirArea reservoirArea)
        {
            Guard.Against.Null(reservoirArea,nameof(reservoirArea));
            Guard.Against.Zero(reservoirArea.WarehouseId, nameof(reservoirArea.WarehouseId));
            await this._reservoirAreaRepository.AddAsync(reservoirArea);
        }

        public async Task AssignLocation(int areaId, List<int> locationIds)
        {
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.NullOrEmpty(locationIds, nameof(locationIds));
            LocationSpecification locationSpec = new LocationSpecification(null,null,null,
                null, null, null, null,  null, null,null,
                 null,null,null);
            var locations = await this._locationRepository.ListAsync(locationSpec);
            List<Location> updLocations=new List<Location>();
            locationIds.ForEach(async (id) =>
            {
                var location = locations.Find(l=>l.Id==id);
                location.ReservoirAreaId = areaId;
                updLocations.Add(location);
            });
            await this._locationRepository.UpdateAsync(updLocations);
        }

        public async Task AssignMaterialType(int wareHouseId, int areaId, List<int> materialDicTypeIds)
        {
            Guard.Against.Zero(wareHouseId, nameof(wareHouseId));
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.NullOrEmpty(materialDicTypeIds, nameof(materialDicTypeIds));
            List<MaterialDicTypeArea> materialDicTypeAreas=new List<MaterialDicTypeArea>();
            materialDicTypeIds.ForEach(async (tId) =>
            {
                MaterialDicTypeArea areaMaterial = new MaterialDicTypeArea
                {
                    WarehouseId = wareHouseId,
                    ReservoirAreaId = areaId,
                    MaterialTypeId = tId
                };
                materialDicTypeAreas.Add(areaMaterial);
            });
            await this._areaMaterialRepository.AddAsync(materialDicTypeAreas);
        }

        public async Task Disable(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            var areaSpec = new ReservoirAreaSpecification(id,null,null, null,null, null);
            var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
            Guard.Against.NullOrEmpty(areas,nameof(areas));
            var area = areas[0];
            area.Status = Convert.ToInt32(AREA_STATUS.禁用);
            await this._reservoirAreaRepository.UpdateAsync(area);
        }

        public async Task Enable(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            var areaSpec = new ReservoirAreaSpecification(id,null,null,null, null, null);
            var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
            Guard.Against.NullOrEmpty(areas, nameof(areas));
            var area = areas[0];
            area.Status = Convert.ToInt32(AREA_STATUS.正常);
            await this._reservoirAreaRepository.UpdateAsync(area);
        }

        public async Task UpdateArea(int id, string areaName)
        {
            Guard.Against.Zero(id, nameof(id));
            var areaSpec = new ReservoirAreaSpecification(id,null,null,null, null, null);
            var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
            Guard.Against.NullOrEmpty(areas, nameof(areas));
            var area = areas[0];
            area.AreaName = areaName;
            await this._reservoirAreaRepository.UpdateAsync(area);
        }
    }
}
