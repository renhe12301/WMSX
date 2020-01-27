using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.ViewModels.BasicInformation;

namespace Web.Services
{
    public class LocationViewModelService:ILocationViewModelService
    {
        private readonly ILocationService _locationService;
        private readonly IAsyncRepository<Location> _locationRepository;

        public LocationViewModelService(ILocationService locationService,
                                        IAsyncRepository<Location> locationRepository)
        {
            this._locationService = locationService;
            this._locationRepository = locationRepository;
        }
        
        
        public async Task<ResponseResultViewModel> GetLocations(int? pageIndex, int? itemsPage,int? id, 
            string sysCode,string userCode,int? orgId,int? ouId, int? wareHouseId,int? areaId,string types,string status,string inStocks)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                
                BaseSpecification<Location> baseSpecification = null;
                List<int> lTypes = null;
                if (!string.IsNullOrEmpty(types))
                {
                    lTypes = types.Split(new char[]{
                        ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                List<int> lStatuss = null;
                if (!string.IsNullOrEmpty(status))
                {
                    lStatuss = status.Split(new char[]{
                        ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                List<int> lInStocks = null;
                if (!string.IsNullOrEmpty(inStocks))
                {
                    lInStocks = inStocks.Split(new char[]{
                        ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new LocationPaginatedSpecification(pageIndex.Value,itemsPage.Value,id,sysCode,userCode,orgId,
                        ouId,wareHouseId,areaId,lTypes,lStatuss,lInStocks);
                }
                else
                {
                    baseSpecification = new LocationSpecification(id,sysCode,userCode,orgId,
                        ouId,wareHouseId,areaId,lTypes,lStatuss,lInStocks);
                }

                var locations = await this._locationRepository.ListAsync(baseSpecification);
                List<LocationViewModel> locationViewModels = new List<LocationViewModel>();

                locations.ForEach(e =>
                {
                    LocationViewModel locationViewModel = new LocationViewModel
                    {
                        Id = e.Id,
                        SysCode = e.SysCode,
                        UserCode = e.UserCode,
                        CreateTime = e.CreateTime.ToString(),
                        ReservoirAreaName = e.ReservoirArea.AreaName,
                        OUName = e.OU.OUName,
                        OrgName = e.Organization.OrgName,
                        WarehouseName = e.Warehouse.WhName,
                        Status =  Enum.GetName(typeof(LOCATION_STATUS), e.Status),
                        InStock = Enum.GetName(typeof(LOCATION_INSTOCK), e.InStock),
                        Type =  Enum.GetName(typeof(LOCATION_TYPE), e.Type)
                    };
                    locationViewModels.Add(locationViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._locationRepository.CountAsync(new LocationSpecification(id,sysCode,userCode,orgId,
                        ouId,wareHouseId,areaId,lTypes,lStatuss,lInStocks));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = locationViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = locationViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
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
