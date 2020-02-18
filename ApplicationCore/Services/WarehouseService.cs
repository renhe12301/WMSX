using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task AddWarehouse(Warehouse warehouse,bool unique=false)
        {
            Guard.Against.Null(warehouse, nameof(warehouse));
            Guard.Against.Zero(warehouse.Id, nameof(warehouse.Id));
            Guard.Against.NullOrEmpty(warehouse.WhName, nameof(warehouse.WhName));
            Guard.Against.NullOrEmpty(warehouse.WhCode, nameof(warehouse.WhCode));
            if (unique)
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(warehouse.Id, null, null,null);
                List<Warehouse> warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                if (warehouses.Count == 0)
                    await this._warehouseRepository.AddAsync(warehouse);
            }
            else
            {
                await this._warehouseRepository.AddAsync(warehouse);
            }
        }
        

        public async Task Disable(int id)
        {
            var wareHouseSpec = new WarehouseSpecification(id,null,null,null);
            var wareHouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
            Guard.Against.NullOrEmpty(wareHouses, nameof(wareHouses));
            var wareHouse = wareHouses[0];
            wareHouse.Status = Convert.ToInt32(WAREHOUSE_STATUS.禁用);
        }

        public async Task Enable(int id)
        {
            var wareHouseSpec = new WarehouseSpecification(id,null,null,null);
            var wareHouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
            Guard.Against.NullOrEmpty(wareHouses, nameof(wareHouses));
            var wareHouse = wareHouses[0];
            wareHouse.Status = Convert.ToInt32(WAREHOUSE_STATUS.正常);
        }

        public async Task UpdateWarehouse(Warehouse warehouse)
        {
            Guard.Against.Null(warehouse, nameof(warehouse));
            await this._warehouseRepository.UpdateAsync(warehouse);
        }

        public async Task AddWarehouse(List<Warehouse> warehouses,bool unique=false)
        {
            Guard.Against.Null(warehouses, nameof(warehouses));
            if (unique)
            {
                List<Warehouse> adds=new List<Warehouse>();
                warehouses.ForEach(async (w) =>
                {
                    var wareHouseSpec = new WarehouseSpecification(w.Id,null,null,null);
                    var wareHouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
                    if (wareHouses.Count > 0)
                        adds.Add(wareHouses.First());
                });
                if (adds.Count > 0)
                    await this._warehouseRepository.AddAsync(adds);
            }
            else
            {
                await this._warehouseRepository.AddAsync(warehouses);
            }

           
        }
        
        public async Task UpdateWarehouse(List<Warehouse> warehouses)
        {
            Guard.Against.Null(warehouses, nameof(warehouses));
            await this._warehouseRepository.UpdateAsync(warehouses);
        }
    }
}
