﻿using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.StockManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using ApplicationCore.Entities.StockManager;
using System.Collections.Generic;
using System.Dynamic;
using ApplicationCore.Misc;
using System.Linq;

namespace Web.Services
{
    public class WarehouseMaterialViewModelService:IWarehouseMaterialViewModelService
    {

        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;

        public WarehouseMaterialViewModelService(IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository)
        {
            this._warehouseMaterialRepository = warehouseMaterialRepository;
        }

        public async Task<ResponseResultViewModel> GetMaterials(int? pageIndex,
            int? itemsPage, int? id, string materialCode, int? materialDicId, string trayCode,string materialName,string materialSpec,
            int? trayDicId,int? orderId,int? orderRowId , int? carrier, string traySteps, int? locationId,
            int? orgId,int? ouId,int? wareHouseId, int? areaId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                List<int> trayStatus = null;
                if (traySteps != null)
                {
                    trayStatus = traySteps.Split(new char[]{
                     ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                BaseSpecification<WarehouseMaterial> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new WarehouseMaterialPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,materialCode,materialDicId,materialName,materialSpec,trayCode,trayDicId,orderId,orderRowId,
                        carrier, trayStatus, locationId,ouId,wareHouseId,areaId);
                }
                else
                {
                    baseSpecification = new WarehouseMaterialSpecification(id, materialCode, materialDicId,
                        materialName,materialSpec,trayCode, trayDicId, orderId,orderRowId, carrier, trayStatus,
                        locationId,ouId, wareHouseId, areaId);
                }

                var materials = await this._warehouseMaterialRepository.ListAsync(baseSpecification);
                List<WarehouseMaterialViewModel> warehouseMaterialViewModels = new List<WarehouseMaterialViewModel>();

                materials.ForEach(e =>
                {
                    WarehouseMaterialViewModel warehouseMaterialViewModel = new WarehouseMaterialViewModel
                    {
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        Carrier = Enum.GetName(typeof(TRAY_CARRIER), e.Carrier),
                        Code = e.MaterialDic.MaterialCode,
                        Img = e.MaterialDic.Img,
                        LocationId = e.LocationId,
                        LocationCode = e.Location.SysCode,
                        MaterialCount = e.MaterialCount,
                        MaterialName = e.MaterialDic.MaterialName,
                        Spec = e.MaterialDic.Spec,
                        ReservoirAreaName = e.ReservoirArea.AreaName,
                        TrayCode = e.WarehouseTray.TrayCode,
                        WarehouseName = e.Warehouse.WhName,
                        OUName = e.OU.OUName

                    };
                    warehouseMaterialViewModels.Add(warehouseMaterialViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._warehouseMaterialRepository.CountAsync(new WarehouseMaterialSpecification(id, materialCode, materialDicId,
                        materialName,materialSpec,trayCode, trayDicId, orderId,orderRowId, carrier, trayStatus,
                        locationId,ouId, wareHouseId, areaId));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = warehouseMaterialViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = warehouseMaterialViewModels;
                }
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
