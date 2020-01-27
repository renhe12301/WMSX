using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface ILocationService
    {
        Task AddLocation( Location location);
        Task BuildLocation(int orgId,int row,int rank,int col);
        Task Enable(int id);
        Task Disable(int id);
        Task Clear(int id);
        Task Lock(int id);
        Task UnLock(int id);
        Task UpdateLocation(int id, string userCode);
    }
}
