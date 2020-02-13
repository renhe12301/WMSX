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
    public class PhyWarehouseViewModelService:IPhyWarehouseViewModelService
    {

        private IAsyncRepository<PhyWarehouse> _phyWarehouseRepository;
        
        public PhyWarehouseViewModelService(IAsyncRepository<PhyWarehouse> phyWarehouseRepository)
        {
            this._phyWarehouseRepository = phyWarehouseRepository;
        }

        public async Task<ResponseResultViewModel> GetPhyWarehouses(int? pageIndex, int? itemsPage, int? id, string phyName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<PhyWarehouse> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new PhyWarehousePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,phyName);
                }
                else
                {
                    baseSpecification = new PhyWarehouseSpecification(id, phyName);
                }
                var warehouses = await this._phyWarehouseRepository.ListAsync(baseSpecification);
                List<PhyWarehouseViewModel> warehouseViewModels = new List<PhyWarehouseViewModel>();
                warehouses.ForEach(e =>
                {
                    PhyWarehouseViewModel wareHouseViewModel = new PhyWarehouseViewModel
                    {
                        Id = e.Id,
                        PhyName = e.PhyName
                    };
                    warehouseViewModels.Add(wareHouseViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._phyWarehouseRepository.CountAsync(new PhyWarehouseSpecification(id, phyName));
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

        public async Task<ResponseResultViewModel> GetPhyWarehouseTrees()
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var all = await this._phyWarehouseRepository.ListAllAsync();
                TreeViewModel current = new TreeViewModel
                {
                    Id = 0,
                    ParentId = 0,
                    Name = "物理仓库",
                    Type = "warehouse"
                };
                all.ForEach(cw =>
                {
                    var wareHouseChild = new TreeViewModel
                    {
                        Id = cw.Id,
                        ParentId = current.Id,
                        Name = cw.PhyName,
                        Type = "warehouse"
                    };
                    current.Children.Add(wareHouseChild);
                });
                response.Data = current;
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