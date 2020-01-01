using System;
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
        Task<ResponseResultViewModel> Lock(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> UnLock(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> UpdateLocation(LocationViewModel locationViewModel);
    }
}
