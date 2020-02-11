﻿using System;
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
        public ReservoirAreaViewModelService(IReservoirAreaService reservoirAreaService,
                                             IAsyncRepository<ReservoirArea> reservoirAreaRepository)
        {
            this._reservoirAreaService = reservoirAreaService;
            this._reservoirAreaRepository = reservoirAreaRepository;
        }


        public async Task<ResponseResultViewModel> GetAreas(int? pageIndex, int? itemsPage, int? id, 
             int? ouId, int? wareHouseId,int? type, string areaName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<ReservoirArea> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new ReservoirAreaPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,ouId,wareHouseId,areaName);
                }
                else
                {
                    baseSpecification = new ReservoirAreaSpecification(id,null,ouId,wareHouseId,type, areaName);
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
                        AreaCode= e.AreaCode,
                        OUName = e.OU.OUName,
                        Status = Enum.GetName(typeof(AREA_STATUS), e.Status)
                    };
                    areaViewModels.Add(areaViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._reservoirAreaRepository.CountAsync(new ReservoirAreaSpecification(id,
                        null,ouId,wareHouseId,type, areaName));
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
        
        
    }
}
