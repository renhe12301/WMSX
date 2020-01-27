using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using Web.ViewModels;
using Web.Interfaces;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Specifications;
using ApplicationCore.Misc;

namespace Web.Services
{
    public class ReservoirAreaViewModelService:IReservoirAreaViewModelService
    {
        private readonly IReservoirAreaService _reservoirAreaService;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<MaterialDicTypeArea> _materialDicTypeArea;
        public ReservoirAreaViewModelService(IReservoirAreaService reservoirAreaService,
                                             IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                                             IAsyncRepository<MaterialDicTypeArea> materialDicTypeArea)
        {
            this._reservoirAreaService = reservoirAreaService;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._materialDicTypeArea = materialDicTypeArea;
        }


        public async Task<ResponseResultViewModel> GetAreas(int? pageIndex, int? itemsPage, int? id, 
            int? orgId, int? ouId, int? wareHouseId,int? type, string areaName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<ReservoirArea> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new ReservoirAreaPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,orgId,ouId,wareHouseId,areaName);
                }
                else
                {
                    baseSpecification = new ReservoirAreaSpecification(id,orgId,ouId,wareHouseId,type, areaName);
                }
                var areas = await this._reservoirAreaRepository.ListAsync(baseSpecification);
                List<ReservoirAreaViewModel> areaViewModels = new List<ReservoirAreaViewModel>();
                areas.ForEach(e =>
                {
                    ReservoirAreaViewModel areaViewModel = new ReservoirAreaViewModel
                    {
                        AreaName = e.AreaName,
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        TypeName = Enum.GetName(typeof(AREA_TYPE), e.Type),
                        WarehouseId = e.WarehouseId,
                        WarehouseName = e.Warehouse.WhName,
                        OrgName = e.Organization.OrgName,
                        OUName = e.OU.OUName,
                        Status = Enum.GetName(typeof(AREA_STATUS), e.Status)
                    };
                    areaViewModels.Add(areaViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._reservoirAreaRepository.CountAsync(new ReservoirAreaSpecification(id,orgId,ouId,wareHouseId,type, areaName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = areaViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = areaViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }
        
        public async Task<ResponseResultViewModel> GetMaterialTypes(int? pageIndex, int? itemsPage,int? areaId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<MaterialDicTypeArea> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new MaterialDicTypeAreaPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        null,areaId,null);
                }
                else
                {
                    baseSpecification = new MaterialDicTypeAreaSpecification(null,areaId,null);
                }

                var materialDicTypes = await this._materialDicTypeArea.ListAsync(baseSpecification);
                List<MaterialTypeViewModel> materialTypeViewModels = new List<MaterialTypeViewModel>();

                materialDicTypes.ForEach(e =>
                {
                    MaterialTypeViewModel materialTypeViewModel = new MaterialTypeViewModel
                    {
                        Id = e.Id,
                        CreateTime = e.MaterialType.CreateTime.ToString(),
                        Memo = e.MaterialType.Memo,
                        TypeName = e.MaterialType.TypeName,
                        ParentId =e.MaterialType.ParentId,
                        AreaName = e.ReservoirArea.AreaName
                    };
                    materialTypeViewModels.Add(materialTypeViewModel);
                });
                materialTypeViewModels.RemoveAll(m => m.ParentId == 0);
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._materialDicTypeArea.CountAsync(new MaterialDicTypeAreaSpecification(null,areaId,null));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = materialTypeViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = materialTypeViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }


        public async Task<ResponseResultViewModel> AddArea(ReservoirAreaViewModel reservoirAreaViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                ReservoirArea reservoirArea = new ReservoirArea
                {
                    AreaName = reservoirAreaViewModel.AreaName,
                    CreateTime = DateTime.Now,
                    WarehouseId = reservoirAreaViewModel.WarehouseId,
                    Type = reservoirAreaViewModel.Type
                };
                await this._reservoirAreaService.AddArea(reservoirArea);
                response.Data = reservoirArea.Id;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> AssignLocation(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                if(!locationViewModel.ReservoirAreaId.HasValue)throw new Exception("分配货位,库区编号不能为空！");
                await this._reservoirAreaService.AssignLocation(locationViewModel.ReservoirAreaId.Value, locationViewModel.LocationIds);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> AssignMaterialType(MaterialDicTypeAreaViewModel materialDicTypeAreaViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._reservoirAreaService.AssignMaterialType(materialDicTypeAreaViewModel.WarehouseId,
                      materialDicTypeAreaViewModel.ReservoirAreaId, materialDicTypeAreaViewModel.MaterialTypeIds);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Disable(ReservoirAreaViewModel reservoirAreaViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._reservoirAreaService.Disable(reservoirAreaViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> Enable(ReservoirAreaViewModel reservoirAreaViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._reservoirAreaService.Enable(reservoirAreaViewModel.Id);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> UpdateArea(ReservoirAreaViewModel reservoirAreaViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._reservoirAreaService.UpdateArea(reservoirAreaViewModel.Id,
                    reservoirAreaViewModel.AreaName);
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
