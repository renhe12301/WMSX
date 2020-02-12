using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IWarehouseService
    {
        Task AddWarehouse(Warehouse warehouse);
        Task UpdateWarehouse(Warehouse warehouse);
        Task Disable(int id);
        Task Enable(int id);
    }
}
