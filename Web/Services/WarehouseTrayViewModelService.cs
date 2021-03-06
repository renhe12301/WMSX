﻿using System;
using Web.Interfaces;
using Web.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Specifications;
using System.Threading.Tasks;
using Web.ViewModels.StockManager;
using System.Collections.Generic;
using System.Dynamic;
using ApplicationCore.Misc;
using System.Linq;

namespace Web.Services
{
    public class WarehouseTrayViewModelService : IWarehouseTrayViewModelService
    {

        private IAsyncRepository<WarehouseTray> _warehouseTrayRepository;

        public WarehouseTrayViewModelService(IAsyncRepository<WarehouseTray> warehouseTrayRepository)
        {
            this._warehouseTrayRepository = warehouseTrayRepository;
        }

        public async Task<ResponseResultViewModel> GetTrays(int? pageIndex, int? itemsPage,
            int? id, string trayCode, string rangeMaterialCount,
            int? subOrderId, int? subOrderRowId, int? carrier,
            string trayTaskStatus, int? locationId, int? ouId, int? wareHouseId, int? areaId,int? pyId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel {Code = 200};

            try
            {
                BaseSpecification<WarehouseTray> baseSpecification = null;
                List<int> trayStatus = null;
                if (!string.IsNullOrEmpty(trayTaskStatus))
                {
                    trayStatus = trayTaskStatus.Split(new char[]
                    {
                        ','
                    }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }

                List<double> trayMaterilCount = null;
                if (!string.IsNullOrEmpty(rangeMaterialCount))
                {
                    trayMaterilCount = rangeMaterialCount.Split(new char[]
                    {
                        ','
                    }, StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToList();

                }

                if (pageIndex.HasValue && itemsPage.HasValue && pageIndex > -1 && itemsPage > 0)
                {

                    baseSpecification = new WarehouseTrayPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, trayCode, trayMaterilCount, subOrderId, subOrderRowId,
                        carrier, trayStatus, locationId, ouId, wareHouseId, areaId,pyId);
                }
                else
                {
                    baseSpecification = new WarehouseTraySpecification(id,trayCode, trayMaterilCount, subOrderId, subOrderRowId,
                        carrier, trayStatus, locationId, ouId, wareHouseId, areaId,pyId);
                }

                var trays = await this._warehouseTrayRepository.ListAsync(baseSpecification);
                List<WarehouseTrayViewModel> warehouseTrayViewModels = new List<WarehouseTrayViewModel>();

                trays.ForEach(e =>
                {
                    WarehouseTrayViewModel warehouseTrayViewModel = new WarehouseTrayViewModel();
                    warehouseTrayViewModel.Id = e.Id;
                    warehouseTrayViewModel.SubOrderId = e.SubOrderId;
                    warehouseTrayViewModel.SubOrderRowId = e.SubOrderRowId;
                    warehouseTrayViewModel.CreateTime = e.CreateTime.ToString();
                    if(e.Carrier.HasValue)
                        warehouseTrayViewModel.Carrier = Enum.GetName(typeof(TRAY_CARRIER), e.Carrier);
                    warehouseTrayViewModel.TrayCode = e.TrayCode;
                    warehouseTrayViewModel.LocationCode = e.Location?.SysCode;
                    warehouseTrayViewModel.MaterialCount = e.MaterialCount;
                    warehouseTrayViewModel.ReservoirAreaName = e.ReservoirArea?.AreaName;
                    warehouseTrayViewModel.WarehouseName = e.Warehouse?.WhName;
                    warehouseTrayViewModel.OUName = e.OU?.OUName;
                    warehouseTrayViewModel.TrayStep = e.TrayStep;
                    if(e.TrayStep.HasValue)
                       warehouseTrayViewModel.TrayStepStr = Enum.GetName(typeof(TRAY_STEP), e.TrayStep);
                    warehouseTrayViewModel.OUId = e.OUId;
                    warehouseTrayViewModel.WarehouseId = e.WarehouseId;
                    warehouseTrayViewModel.CargoHeight = e.CargoHeight;
                    warehouseTrayViewModel.CargoWeight = e.CargoWeight;
                    warehouseTrayViewModel.LocationId = e.LocationId;
                    warehouseTrayViewModel.OutCount = e.OutCount.GetValueOrDefault();
                    warehouseTrayViewModel.PhyWarehouseId = e.PhyWarehouseId;
                    warehouseTrayViewModel.PhyName = e.PhyWarehouse?.PhyName;
                    warehouseTrayViewModels.Add(warehouseTrayViewModel);
                });

                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._warehouseTrayRepository.CountAsync(new WarehouseTraySpecification(id,
                        trayCode, trayMaterilCount, subOrderId, subOrderRowId, carrier, trayStatus, locationId, ouId, wareHouseId, areaId,pyId));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = warehouseTrayViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = warehouseTrayViewModels;
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
