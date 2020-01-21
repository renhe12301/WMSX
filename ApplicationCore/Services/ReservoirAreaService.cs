using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly ITransactionRepository _transactionRepository;
        public ReservoirAreaService(IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                                    IAsyncRepository<Location> locationRepository,
                                    IAsyncRepository<MaterialDicTypeArea> areaMaterialRepository,
                                    ITransactionRepository transactionRepository)
        {
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._locationRepository = locationRepository;
            this._areaMaterialRepository = areaMaterialRepository;
            this._transactionRepository = transactionRepository;
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
            this._transactionRepository.Transaction(() =>
            {
                locationIds.ForEach(async (id) =>
                {
                    LocationSpecification locationSpec = new LocationSpecification(id,null,null, null, null, null, null, null, null);
                    var locations = await this._locationRepository.ListAsync(locationSpec);
                    var location = locations[0];
                    location.ReservoirAreaId = areaId;
                    await this._locationRepository.UpdateAsync(location);
                });
            });
        }

        public async Task AssignMaterialType(int wareHouseId, int areaId, List<int> materialDicTypeIds)
        {
            Guard.Against.Zero(wareHouseId, nameof(wareHouseId));
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.NullOrEmpty(materialDicTypeIds, nameof(materialDicTypeIds));
            this._transactionRepository.Transaction(() =>
            {
                materialDicTypeIds.ForEach(async (tId) =>
                {
                    MaterialDicTypeArea areaMaterial = new MaterialDicTypeArea
                    {
                        WarehouseId = wareHouseId,
                        ReservoidAreaId = areaId,
                        MaterialTypeId = tId
                    };
                    await this._areaMaterialRepository.AddAsync(areaMaterial);
                });
            });
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
