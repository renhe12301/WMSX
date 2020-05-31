using System;
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
            int? orderId, int? orderRowId, int? carrier,
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

                List<int> trayMaterilCount = null;
                if (!string.IsNullOrEmpty(rangeMaterialCount))
                {
                    trayMaterilCount = rangeMaterialCount.Split(new char[]
                    {
                        ','
                    }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }

                if (pageIndex.HasValue && itemsPage.HasValue && pageIndex > -1 && itemsPage > 0)
                {

                    baseSpecification = new WarehouseTrayPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, trayCode, trayMaterilCount, orderId, orderRowId,
                        carrier, trayStatus, locationId, ouId, wareHouseId, areaId,pyId);
                }
                else
                {
                    baseSpecification = new WarehouseTraySpecification(id,trayCode, trayMaterilCount, orderId, orderRowId,
                        carrier, trayStatus, locationId, ouId, wareHouseId, areaId,pyId);
                }

                var trays = await this._warehouseTrayRepository.ListAsync(baseSpecification);
                List<WarehouseTrayViewModel> warehouseTrayViewModels = new List<WarehouseTrayViewModel>();

                trays.ForEach(e =>
                {
                    WarehouseTrayViewModel warehouseTrayViewModel = new WarehouseTrayViewModel();
                    warehouseTrayViewModel.Id = e.Id;
                    warehouseTrayViewModel.CreateTime = e.CreateTime.ToString();
                    warehouseTrayViewModel.Carrier = Enum.GetName(typeof(TRAY_CARRIER), e.Carrier);
                    warehouseTrayViewModel.TrayCode = e.TrayCode;
                    warehouseTrayViewModel.LocationCode = e.Location?.SysCode;
                    warehouseTrayViewModel.MaterialCount = e.MaterialCount;
                    warehouseTrayViewModel.ReservoirAreaName = e.ReservoirArea?.AreaName;
                    warehouseTrayViewModel.WarehouseName = e.Warehouse?.WhName;
                    warehouseTrayViewModel.OUName = e.OU?.OUName;
                    warehouseTrayViewModel.TrayStepStr = Enum.GetName(typeof(TRAY_STEP), e.TrayStep);
                    warehouseTrayViewModels.Add(warehouseTrayViewModel);
                });

                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._warehouseTrayRepository.CountAsync(new WarehouseTraySpecification(id,
                        trayCode, trayMaterilCount, orderId, orderRowId, carrier, trayStatus, locationId, ouId, wareHouseId, areaId,pyId));
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
