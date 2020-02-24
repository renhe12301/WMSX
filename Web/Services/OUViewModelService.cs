using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Dynamic;

namespace Web.Services
{
    public class OUViewModelService:IOUViewModelService
    {

        private IAsyncRepository<OU> _ouRepository;
        private IAsyncRepository<Warehouse> _warehouseRepository;
        private IAsyncRepository<ReservoirArea> _areaRepository;

        public OUViewModelService(IAsyncRepository<OU> ouRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<ReservoirArea> areaRepository)
        {
            this._ouRepository = ouRepository;
            this._warehouseRepository = warehouseRepository;
            this._areaRepository = areaRepository;
        }

        public async Task<ResponseResultViewModel> GetOUs(int? pageIndex, int? itemsPage, int? id, string ouName, string ouCode,string companyName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<OU> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new OUPaginatedSpecification(pageIndex.Value,
                        itemsPage.Value, id, ouName, ouCode,companyName);
                }
                else
                {
                    baseSpecification = new OUSpecification(id,ouName,ouCode,companyName);
                }

                var result = await this._ouRepository.ListAsync(baseSpecification);
                List<OUViewModel> ouViewModels = new List<OUViewModel>();
                result.ForEach(r =>
                {
                    OUViewModel ouViewModel = new OUViewModel
                    {
                        CompanyName = r.CompanyName,
                        OUName = r.OUName,
                        OUCode = r.OUCode,
                        PlateCode = r.PlateCode,
                        PlateName = r.PlateName,
                        Id = r.Id
                    };
                    ouViewModels.Add(ouViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._ouRepository.CountAsync(new OUSpecification(id, ouName, ouCode,companyName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = ouViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = ouViewModels;
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }
        
          public async Task<ResponseResultViewModel> GetOUTrees(int rootId,string depthTag)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var ouSpec = new OUSpecification(null, null, null,null);
                var allOus =  await this._ouRepository.ListAsync(ouSpec);
                if (allOus.Count == 0) throw new Exception(string.Format("业务实体不存在"));
                var warehouseSpec=new WarehouseSpecification(null,null,null,null);
                var warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                var areaSpec=new ReservoirAreaSpecification(null,null,null,null,null,null);
                var areas = await this._areaRepository.ListAsync(areaSpec);
                
                TreeViewModel current =null;
                if (rootId > 0)
                {
                    var ou = allOus.Find(o => o.Id == rootId);
                    current = new TreeViewModel
                    {
                        Id = ou.Id,
                        ParentId = 0,
                        Name = ou.OUName,
                        Type = "ou"
                    };
                }
                else
                {
                    current = new TreeViewModel
                    {
                        Id = 0,
                        ParentId = 0,
                        Name = "业务实体",
                        Type = "ou"
                    };
                }

                if (depthTag == "ou")
                {
                    allOus.ForEach(cw =>
                    {
                        var ouChild = new TreeViewModel
                        {
                            Id = cw.Id,
                            ParentId = current.Id,
                            Name = cw.OUName,
                            Type = "ou"
                        };
                        current.Children.Add(ouChild);
                    });
                }
                else if (depthTag == "warehouse")
                {
                    if (rootId > 0)
                    {
                        var childWarehouses = warehouses.FindAll(w => w.OUId == current.Id);
                        childWarehouses.ForEach(cw =>
                        {
                            var wareHouseChild = new TreeViewModel
                            {
                                Id = cw.Id,
                                ParentId = current.Id,
                                Name = cw.WhName,
                                Type = "warehouse"
                            };
                            current.Children.Add(wareHouseChild);
                        });
                    }
                    else
                    {
                        allOus.ForEach(ou =>
                        {
                            var ouChild = new TreeViewModel
                            {
                                Id = ou.Id,
                                ParentId = 0,
                                Name = ou.OUName,
                                Type = "ou"
                            };
                            var childWarehouses = warehouses.FindAll(w => w.OUId == ou.Id);
                            childWarehouses.ForEach(cw =>
                            {
                                var wareHouseChild = new TreeViewModel
                                {
                                    Id = cw.Id,
                                    ParentId = current.Id,
                                    Name = cw.WhName,
                                    Type = "warehouse"
                                };
                                ouChild.Children.Add(wareHouseChild);
                            });
                            
                            current.Children.Add(ouChild);
                        });
                    }
                }
                else if (depthTag == "area")
                {
                    if (rootId > 0)
                    {
                        var childWarehouses = warehouses.FindAll(w=>w.OUId==current.Id);
                        childWarehouses.ForEach(async (cw) =>
                        {
                            var wareHouseChild = new TreeViewModel
                            {
                                Id = cw.Id,
                                ParentId = current.Id,
                                Name = cw.WhName,
                                Type = "warehouse"
                            };
                            var childAreas = areas.FindAll(a=>a.WarehouseId==cw.Id);
                            childAreas.ForEach(async (ca) =>
                            {
                                var areaChild = new TreeViewModel
                                {
                                    Id = ca.Id,
                                    ParentId = cw.Id,
                                    Name = ca.AreaName,
                                    Type = "area"
                                };
                                wareHouseChild.Children.Add(areaChild);
                            });
                            current.Children.Add(wareHouseChild);
                        });
                    }
                    else
                    {
                        allOus.ForEach(ou =>
                        {
                            var ouChild = new TreeViewModel
                            {
                                Id = ou.Id,
                                ParentId = 0,
                                Name = ou.OUName,
                                Type = "ou"
                            };
                            var childWarehouses = warehouses.FindAll(w=>w.OUId==ou.Id);
                            childWarehouses.ForEach(async (cw) =>
                            {
                                var wareHouseChild = new TreeViewModel
                                {
                                    Id = cw.Id,
                                    ParentId = current.Id,
                                    Name = cw.WhName,
                                    Type = "warehouse"
                                };
                                var childAreas = areas.FindAll(a=>a.WarehouseId==cw.Id);
                                childAreas.ForEach(async (ca) =>
                                {
                                    var areaChild = new TreeViewModel
                                    {
                                        Id = ca.Id,
                                        ParentId = cw.Id,
                                        Name = ca.AreaName,
                                        Type = "area"
                                    };
                                    wareHouseChild.Children.Add(areaChild);
                                });
                                ouChild.Children.Add(wareHouseChild);
                            });
                            current.Children.Add(ouChild);
                        });
                        
                    }
                }

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
