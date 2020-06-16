using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using System.Dynamic;
using System.Linq;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.TaskManager;

namespace Web.Services
{
    public class WarehouseViewModelService : IWarehouseViewModelService
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IAsyncRepository<Warehouse> _wareHouseRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<Order> _orderRepository;
        public WarehouseViewModelService(IWarehouseService warehouseService,
                                         IAsyncRepository<Warehouse> wareHouseRepository,
                                         IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                                         IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                                         IAsyncRepository<InOutTask> inOutTaskRepository,
                                         IAsyncRepository<Order> orderRepository
                                         )
        {
            this._warehouseService = warehouseService;
            this._wareHouseRepository = wareHouseRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._inOutTaskRepository = inOutTaskRepository;
            this._orderRepository = orderRepository;
        }

        public async Task<ResponseResultViewModel> GetWarehouses(int? pageIndex,
            int? itemsPage, int? id,int? ouId, string whName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Warehouse> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new WarehousePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,ouId,whName);
                }
                else
                {
                    baseSpecification = new WarehouseSpecification(id, ouId, whName,null);
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
                        OUId = e.OUId,
                        OUName= e.OU?.OUName,
                        WhCode = e.WhCode,
                        Status = Enum.GetName(typeof(WAREHOUSE_STATUS), e.Status),
                        PhyWarehouseId = e.PhyWarehouseId,
                        PhyName = e.PhyWarehouse?.PhyName
                    };
                    warehouseViewModels.Add(wareHouseViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._wareHouseRepository.CountAsync(new WarehouseSpecification(id, ouId,  whName,null));
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
        
    }
}
