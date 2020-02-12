using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IWarehouseService
    {
        Task AddWarehouse(Warehouse warehouse,bool unique=false);
        Task UpdateWarehouse(Warehouse warehouse);
        Task AddWarehouse(List<Warehouse> warehouses,bool unique=false);
        Task UpdateWarehouse(List<Warehouse> warehouses);
        Task Disable(int id);
        Task Enable(int id);
    }
}
