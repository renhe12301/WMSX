using System;
using Web.Interfaces;
using Web.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Specifications;
using System.Threading.Tasks;
using Web.ViewModels.StockManager;
using System.Collections.Generic;
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

        public async Task<ResponseResultViewModel> GetTrays(int? pageIdex,int? itemsPage,
            int? includeDetail,int? id, string trayCode, string rangeMaterialCount,
            int? orderId,int? orderRowId,int? carrier,
            string trayTaskStatus, int? locationId,int? ouId, int? wareHouseId, int? areaId)
        {
            BaseSpecification<WarehouseTray> baseSpecification = null;
            List<int> trayStatus = null;
            if (!string.IsNullOrEmpty(trayTaskStatus))
            {
                trayStatus = trayTaskStatus.Split(new char[]{
               ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

            }
            List<int> trayMaterilCount = null;
            if (!string.IsNullOrEmpty(rangeMaterialCount))
            {
                trayMaterilCount = rangeMaterialCount.Split(new char[]{
               ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

            }
            if (includeDetail.HasValue && includeDetail > 0)
            {
                if (pageIdex.HasValue && itemsPage.HasValue && pageIdex > 0 && itemsPage > 0)
                {
                    
                    baseSpecification = new WarehouseTrayPaginatedDetailSpecification(pageIdex.Value,itemsPage.Value,id,
                            trayCode, trayMaterilCount, orderId,orderRowId,
                            carrier, trayStatus, locationId,ouId, wareHouseId, areaId);
                }
                else
                {
                    baseSpecification = new WarehouseTrayDetailSpecification(id,
                            trayCode, trayMaterilCount, orderId,orderRowId,
                            carrier, trayStatus, locationId,ouId, wareHouseId, areaId);
                }
            }
            else
            {
                if (pageIdex.HasValue && itemsPage.HasValue && pageIdex > 0 && itemsPage > 0)
                {
                    baseSpecification = new WarehouseTrayPaginatedSpecification(pageIdex.Value, itemsPage.Value, id,
                           trayCode, trayMaterilCount, orderId,orderRowId,
                           carrier, trayStatus, locationId,ouId, wareHouseId, areaId);
                }
                else
                {
                    baseSpecification = new WarehouseTraySpecification(id,
                          trayCode, trayMaterilCount, orderId,orderRowId,
                          carrier, trayStatus, locationId,  ouId, wareHouseId, areaId);
                }
            }
           
            var result = await GetTrayList(includeDetail.HasValue?includeDetail.Value:0, baseSpecification);
            return result;
        }

        async Task<ResponseResultViewModel> GetTrayList(int includeDetail,BaseSpecification<WarehouseTray> baseSpecification)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {

                var result = await this._warehouseTrayRepository.ListAsync(baseSpecification);
                List<WarehouseTrayViewModel> trayViewModels = new List<WarehouseTrayViewModel>();
                result.ForEach((r) =>
                {
                    WarehouseTrayViewModel wtvm = new WarehouseTrayViewModel
                    {
                        Id = r.Id,
                        OrderId=r.OrderId,
                        OrderRowId=r.OrderRowId,
                        Code = r.TrayCode,
                        CreateTime = r.CreateTime.ToString(),
                        LocationId = r.Location.Id,
                        MaterialCount = r.MaterialCount,
                        ReservoirAreaName = r.ReservoirArea.AreaName,
                        WarehouseName = r.Warehouse.WhName,
                        Carrier = Enum.GetName(typeof(TRAY_CARRIER), r.Carrier),
                        TrayStep = Enum.GetName(typeof(TRAY_STEP), r.TrayStep),
                    };
                    trayViewModels.Add(wtvm);
                    if (includeDetail > 0)
                    {
                        r.WarehouseMaterial.ForEach(wm =>
                        {
                            WarehouseMaterialViewModel warehouseMaterialViewModel = new WarehouseMaterialViewModel
                            {
                                Id = wm.Id,
                                OrderId = r.OrderId,
                                OrderRowId = r.OrderRowId,
                                Code = wm.MaterialDic.MaterialCode,
                                CreateTime = wm.CreateTime.ToString(),
                                LocationId = wm.Location.Id,
                                MaterialCount = wm.MaterialCount,
                                Img = wm.MaterialDic.Img,
                                Carrier = Enum.GetName(typeof(TRAY_CARRIER), wm.Carrier),
                                MaterialName = wm.MaterialDic.MaterialName,
                                ReservoirAreaName = wm.ReservoirArea.AreaName,
                                TrayCode = wm.WarehouseTray.TrayCode,
                                WarehouseName = wm.Warehouse.WhName
                            };
                            wtvm.WarehouseMaterialViewModels.Add(warehouseMaterialViewModel);
                        });
                    }

                });
                response.Data = trayViewModels;
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
