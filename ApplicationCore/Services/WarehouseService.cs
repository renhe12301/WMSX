using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class WarehouseService:IWarehouseService
    {
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        public WarehouseService(IAsyncRepository<Warehouse> warehouseRepository)
        {
            this._warehouseRepository = warehouseRepository;
        }

        public async Task AddWarehouse(Warehouse warehouse)
        {
            await this._warehouseRepository.AddAsync(warehouse);
        }

        public async Task Disable(int id)
        {
            var wareHouseSpec = new WarehouseSpecification(id,null,null);
            var wareHouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
            Guard.Against.NullOrEmpty(wareHouses, nameof(wareHouses));
            var wareHouse = wareHouses[0];
            wareHouse.Status = Convert.ToInt32(WAREHOUSE_STATUS.禁用);
        }

        public async Task Enable(int id)
        {
            var wareHouseSpec = new WarehouseSpecification(id,null,null);
            var wareHouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
            Guard.Against.NullOrEmpty(wareHouses, nameof(wareHouses));
            var wareHouse = wareHouses[0];
            wareHouse.Status = Convert.ToInt32(WAREHOUSE_STATUS.正常);
        }

        public async Task UpdateWarehouse(int id, string whName, string address)
        {
            var wareHouseSpec = new WarehouseSpecification(id,null,null);
            var wareHouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
            Guard.Against.NullOrEmpty(wareHouses, nameof(wareHouses));
            var wareHouse = wareHouses[0];
            wareHouse.WhName=whName;
            wareHouse.Address = address;
        }
    }
}
