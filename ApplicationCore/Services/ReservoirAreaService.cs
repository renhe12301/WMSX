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
        private readonly IAsyncRepository<Location> _locationRepository;
        public ReservoirAreaService(IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                                    IAsyncRepository<Location> locationRepository)
        {
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._locationRepository = locationRepository;
        }

        public async Task AddArea(ReservoirArea reservoirArea)
        {
            Guard.Against.Null(reservoirArea, nameof(reservoirArea));
            Guard.Against.Zero(reservoirArea.WarehouseId, nameof(reservoirArea.WarehouseId));
            Guard.Against.NullOrEmpty(reservoirArea.AreaCode, nameof(reservoirArea.AreaCode));
            ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null, reservoirArea.AreaCode,
                null,
                null, null, null);
            List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
            if (areas.Count == 0)
                await this._reservoirAreaRepository.AddAsync(reservoirArea);
        }

        public async Task AssignLocation(int areaId, List<int> locationIds)
        {
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.NullOrEmpty(locationIds, nameof(locationIds));
            LocationSpecification locationSpec = new LocationSpecification(null,null,null,
                null,null, null, null, null,  null, null,null,
                 null,null);
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

        public async Task UpdateArea(ReservoirArea reservoirArea)
        {
            Guard.Against.Null(reservoirArea, nameof(reservoirArea));
            Guard.Against.Zero(reservoirArea.Id, nameof(reservoirArea.Id));
            Guard.Against.Zero(reservoirArea.WarehouseId, nameof(reservoirArea.WarehouseId));
            Guard.Against.NullOrEmpty(reservoirArea.AreaCode, nameof(reservoirArea.AreaCode));
            var areaSpec = new ReservoirAreaSpecification(reservoirArea.Id,null,null,null,null, null);
            var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
            if (areas.Count > 0)
            {
                var area = areas[0];
                area.AreaName = reservoirArea.AreaName;
                area.AreaName = reservoirArea.AreaName;
                area.Status = reservoirArea.Status;
                area.CreateTime = reservoirArea.CreateTime;
                area.EndTime = reservoirArea.EndTime;
                area.WarehouseId = reservoirArea.WarehouseId;
                await this._reservoirAreaRepository.UpdateAsync(area);
            }
        }
    }
}
