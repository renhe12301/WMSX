using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task AddArea(ReservoirArea reservoirArea,bool unique=false)
        {
            Guard.Against.Null(reservoirArea, nameof(reservoirArea));
            Guard.Against.Zero(reservoirArea.Id, nameof(reservoirArea.Id));
            Guard.Against.Zero(reservoirArea.WarehouseId, nameof(reservoirArea.WarehouseId));
            Guard.Against.NullOrEmpty(reservoirArea.AreaCode, nameof(reservoirArea.AreaCode));
            if (unique)
            {
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(reservoirArea.Id, null,
                    null,
                    null, null, null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                if (areas.Count == 0)
                    await this._reservoirAreaRepository.AddAsync(reservoirArea);
            }
            else
            {
                await this._reservoirAreaRepository.AddAsync(reservoirArea);
            }

        }

        public async Task UpdateArea(List<ReservoirArea> reservoirAreas)
        {
            Guard.Against.Null(reservoirAreas, nameof(reservoirAreas));
            await this._reservoirAreaRepository.UpdateAsync(reservoirAreas);
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
            await this._reservoirAreaRepository.UpdateAsync(reservoirArea);
        }

        public async Task AddArea(List<ReservoirArea> reservoirAreas, bool unique = false)
        {
            Guard.Against.NullOrEmpty(reservoirAreas,nameof(reservoirAreas));
            if (unique)
            {
                List<ReservoirArea> adds=new List<ReservoirArea>();
                reservoirAreas.ForEach(async (area) =>
                {
                    ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(area.Id, null,
                        null,
                        null, null, null);
                    List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                    if(areas.Count==0)
                        adds.Add(area);
                });
                if (adds.Count > 0)
                    await this._reservoirAreaRepository.AddAsync(adds);
            }
            else
            {
                await this._reservoirAreaRepository.AddAsync(reservoirAreas);
            }
        }
    }
}
