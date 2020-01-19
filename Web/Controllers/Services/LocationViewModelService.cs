using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using Web.ViewModels.BasicInformation;

namespace Web.Services
{
    public class LocationViewModelService:ILocationViewModelService
    {
        private readonly ILocationService _locationService;


        public LocationViewModelService(ILocationService locationService)
        {
            this._locationService = locationService;
        }

        public async Task<ResponseResultViewModel> AddLocation(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                Location location = new Location
                {
                    CreateTime = DateTime.Now,
                    SysCode = locationViewModel.SysCode,
                    UserCode = locationViewModel.UserCode,
                    WarehouseId=locationViewModel.WarehouseId,
                    ReservoirAreaId=locationViewModel.ReservoirAreaId
                };
                await this._locationService.AddLocation(location);
                response.Data = location.Id;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> BuildLocation(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._locationService.BuildLocation(locationViewModel.WarehouseId,
                    locationViewModel.Row, locationViewModel.Rank, locationViewModel.Col);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Clear(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._locationService.Clear(locationViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Disable(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._locationService.Disable(locationViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Enable(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._locationService.Enable(locationViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Lock(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._locationService.Lock(locationViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> UnLock(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._locationService.UnLock(locationViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> UpdateLocation(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._locationService.UpdateLocation(locationViewModel.Id, locationViewModel.UserCode);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }
    }
}
