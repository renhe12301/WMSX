using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface ILocationViewModelService
    {
        Task<ResponseResultViewModel> AddLocation(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> BuildLocation(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> Enable(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> Disable(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> Clear(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> UpdateLocation(LocationViewModel locationViewModel);

        Task<ResponseResultViewModel> GetLocations(int? pageIndex, int? itemsPage, int? id,
            string sysCode,string userCode,int? phyId,int? ouId, int? wareHouseId, int? areaId, string status,
            string inStocks,string isTasks,string floors,string items,string cols);

        Task<ResponseResultViewModel> GetMaxFloorItemCol(int phyId);
        Task<ResponseResultViewModel> GetMaxFloor(int phyId);
        
        Task<ResponseResultViewModel> GetMaxItem(int phyId);
        
        Task<ResponseResultViewModel> GetMaxCol(int phyId);
        
    }
}
