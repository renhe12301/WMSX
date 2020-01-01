using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Interfaces
{
    public interface IReservoirAreaService
    {
        Task AddArea(ReservoirArea reservoirArea);
        Task UpdateArea(int id, string areaName);
        Task Enable(int id);
        Task Disable(int id);
        Task AssignLocation(int areaId, List<int> locationIds);
        Task AssignMaterialType(int wareHouseId, int areaId, List<int> materialTypeIds);

    }
}
