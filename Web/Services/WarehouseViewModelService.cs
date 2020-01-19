using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Services
{
    public class WarehouseViewModelService : IWarehouseViewModelService
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IAsyncRepository<Warehouse> _wareHouseRepository;

        public WarehouseViewModelService(IWarehouseService warehouseService,
                                         IAsyncRepository<Warehouse> wareHouseRepository)
        {
            this._warehouseService = warehouseService;
            this._wareHouseRepository = wareHouseRepository;
        }

        public async Task<ResponseResultViewModel> GetWarehouses(int? pageIndex,
            int? itemsPage, int? id, int? orgId, string whName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Warehouse> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new WarehousePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,null,orgId,whName);
                }
                else
                {
                    baseSpecification = new WarehouseSpecification(id, null, orgId, whName);
                }
                var warehouses = await this._wareHouseRepository.ListAsync(baseSpecification);
                List<WarehouseViewModel> warehouseViewModels = new List<WarehouseViewModel>();
                warehouses.ForEach(e =>
                {
                    WarehouseViewModel wareHouseViewModel = new WarehouseViewModel
                    {
                        WhName = e.WhName,
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        OrganizationId=e.Organization.Id,
                        OrgName=e.Organization.OrgName,
                        Status = Enum.GetName(typeof(WAREHOUSE_STATUS), e.Status)
                    };
                    warehouseViewModels.Add(wareHouseViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._wareHouseRepository.CountAsync(new WarehouseSpecification(id, null, orgId, whName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = warehouseViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = warehouseViewModels;
                }
               
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> AddWarehouse(WarehouseViewModel warehouseViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                Warehouse warehouse = new Warehouse
                {
                    WhName = warehouseViewModel.WhName,
                    Address = warehouseViewModel.Address,
                    CreateTime = DateTime.Now,
                    OrganizationId = warehouseViewModel.OrganizationId,
                    Memo=warehouseViewModel.Memo
                };
                await this._warehouseService.AddWarehouse(warehouse);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Disable(WarehouseViewModel warehouseViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._warehouseService.Disable(warehouseViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Enable(WarehouseViewModel warehouseViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._warehouseService.Enable(warehouseViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> UpdateWarehouse(WarehouseViewModel warehouseViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._warehouseService.UpdateWarehouse(warehouseViewModel.Id,
                    warehouseViewModel.WhName,warehouseViewModel.Address);
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
