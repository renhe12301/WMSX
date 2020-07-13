using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Interfaces
{
    public interface IReservoirAreaService
    {
        Task AddArea(ReservoirArea reservoirArea,bool unique=false);
        Task UpdateArea(ReservoirArea reservoirArea);
        
        Task AddArea(List<ReservoirArea> reservoirAreas,bool unique=false);
        Task UpdateArea(List<ReservoirArea> reservoirAreas);
        
        Task AssignLocation(int areaId, List<int> locationIds);

        Task SetOwnerType(int areaId, string ownerType);

    }
}
