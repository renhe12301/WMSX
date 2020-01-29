using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface ILocationService
    {
        Task AddLocation( Location location);
        Task BuildLocation(int orgId,int row,int rank,int col);
        Task Enable(List<int> ids);
        Task Disable(List<int> ids);
        Task Clear(List<int> ids);
        Task UpdateLocation(int id, string sysCode,string userCode);
    }
}
