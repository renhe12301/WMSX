using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
            Guard.Against.Null(warehouse, nameof(warehouse));
            Guard.Against.Zero(warehouse.Id, nameof(warehouse.Id));
            Guard.Against.NullOrEmpty(warehouse.WhName, nameof(warehouse.WhName));
            Guard.Against.NullOrEmpty(warehouse.WhCode, nameof(warehouse.WhCode));
            WarehouseSpecification warehouseSpec = new WarehouseSpecification(warehouse.Id, null, null);
            List<Warehouse> warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
            if (warehouses.Count == 0)
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

        public async Task UpdateWarehouse(Warehouse warehouse)
        {
            Guard.Against.Null(warehouse, nameof(warehouse));
            Guard.Against.Zero(warehouse.Id, nameof(warehouse.Id));
            Guard.Against.NullOrEmpty(warehouse.WhName, nameof(warehouse.WhName));
            Guard.Against.NullOrEmpty(warehouse.WhCode, nameof(warehouse.WhCode));
            var wareHouseSpec = new WarehouseSpecification(warehouse.Id,null,null);
            var wareHouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
            if (wareHouses.Count > 0)
            {
                var wareHouse = wareHouses[0];
                wareHouse.Id = warehouse.Id;
                wareHouse.WhName=warehouse.WhName;
                wareHouse.OUId = warehouse.OUId;
                wareHouse.WhCode = warehouse.WhCode;
                wareHouse.CreateTime = warehouse.CreateTime;
                wareHouse.EndTime = warehouse.EndTime;
                await this._warehouseRepository.UpdateAsync(wareHouse);
            }
        }
    }
}
