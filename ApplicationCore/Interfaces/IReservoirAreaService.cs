using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Interfaces
{
    public interface IReservoirAreaService
    {
        Task AddArea(ReservoirArea reservoirArea);
        Task UpdateArea(ReservoirArea reservoirArea);
        
        Task AssignLocation(int areaId, List<int> locationIds);

    }
}
