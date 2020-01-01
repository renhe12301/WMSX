using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IWarehouseService
    {
        Task AddWarehouse(Warehouse warehouse);
        Task UpdateWarehouse(int id,string whName,string address);
        Task Disable(int id);
        Task Enable(int id);
    }
}
