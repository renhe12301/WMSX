using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;
using System.Dynamic;

namespace Web.Services
{
    public class OrganizationViewModelService : IOrganizationViewModelService
    {
        private readonly IOrganizationService _organizationService;
        private readonly IAsyncRepository<Organization> _organizationRepository;
        private readonly IAsyncRepository<OU> _ouRepository;
        private readonly IAsyncRepository<EmployeeOrg> _employeeOrgRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;

        public OrganizationViewModelService(IOrganizationService organizationService,
                                            IAsyncRepository<Organization> organizationRepository,
                                            IAsyncRepository<OU> ouRepository,
                                            IAsyncRepository<EmployeeOrg> employeeOrgRepository,
                                            IAsyncRepository<Warehouse> warehouseRepository,
                                            IAsyncRepository<ReservoirArea> reservoirAreaRepository)
        {
            this._organizationService = organizationService;
            this._organizationRepository = organizationRepository;
            this._ouRepository = ouRepository;
            this._employeeOrgRepository = employeeOrgRepository;
            this._warehouseRepository = warehouseRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
        }

        public async Task<ResponseResultViewModel> GetOrganizationTrees(int rootId,string depthTag)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var orgSpec = new OrganizationSpecification(null, null, null);
                var allOrgs =  await this._organizationRepository.ListAsync(orgSpec);
                if (allOrgs.Count == 0) throw new Exception(string.Format("组织架构不存在"));
                var org = allOrgs.Find(o=>o.Id==rootId);
                var warehouseSpec=new WarehouseSpecification(null,null,null,null);
                var warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                var ouSpec=new OUSpecification(null,null,null,null);
                var ous = await this._ouRepository.ListAsync(ouSpec);
                var areaSpec=new ReservoirAreaSpecification(null,null,null,null,null,null);
                var areas = await this._reservoirAreaRepository.ListAsync(areaSpec);
                TreeViewModel current = new TreeViewModel
                {
                    Id=org.Id,
                    ParentId=org.ParentId,
                    Name=org.OrgName,
                    Type="org"
                };

                if (depthTag == "org")
                {
                    await OrganizationTree(current, current.Children,allOrgs);
                }
                else if (depthTag == "ou")
                {
                    var childOUs = ous.FindAll(ou=>ou.OrganizationId==current.Id);
                    childOUs.ForEach(cw =>
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
                    await Organization2OUTree(current, current.Children,allOrgs,ous);
                }
                else if (depthTag == "warehouse")
                {
                    var childOUs = ous.FindAll(ou=>ou.OrganizationId==current.Id);
                    childOUs.ForEach(async(ou) =>
                    {
                        var ouChild = new TreeViewModel
                        {
                            Id = ou.Id,
                            ParentId = current.Id,
                            Name = ou.OUName,
                            Type = "ou"
                        };
                        var childWarehouses = warehouses.FindAll(w=>w.OUId==ou.Id);
                        childWarehouses.ForEach(cw =>
                        {
                            var wareHouseChild = new TreeViewModel
                            {
                                Id = cw.Id,
                                ParentId = ou.Id,
                                Name = cw.WhName,
                                Type = "warehouse"
                            };
                            ouChild.Children.Add(wareHouseChild);
                        });
                        current.Children.Add(ouChild);
                    });
                    
                    await Organization2WarehouseTree(current, current.Children,allOrgs,ous,warehouses);
                }
                else if (depthTag == "area")
                {

                    var childOUs = ous.FindAll(ou=>ou.OrganizationId==current.Id);
                    childOUs.ForEach(async (ou) =>
                    {
                        var ouChild = new TreeViewModel
                        {
                            Id = ou.Id,
                            ParentId = current.Id,
                            Name = ou.OUName,
                            Type = "ou"
                        };
                        var wareHouseSpec = new WarehouseSpecification(null, null, ou.Id, null);
                        var childWarehouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
                        childWarehouses.ForEach(async(cw) =>
                        {
                            var wareHouseChild = new TreeViewModel
                            {
                                Id = cw.Id,
                                ParentId = ou.Id,
                                Name = cw.WhName,
                                Type = "warehouse"
                            };
                            var areaSpec = new ReservoirAreaSpecification(null, null, null, cw.Id, null, null);
                            var childAreas = await this._reservoirAreaRepository.ListAsync(areaSpec);
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
                    await Organization2AreaTree(current, current.Children,allOrgs,ous,warehouses,areas);
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

        private async Task OrganizationTree(TreeViewModel current, List<TreeViewModel> childs,List<Organization> allOrgs)
        {
            var orgs = allOrgs.FindAll(o=>o.ParentId==current.Id);
            orgs.ForEach(async (corg) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = corg.Id,
                    ParentId = current.Id,
                    Name = corg.OrgName,
                    Type = "org"
                };
                childs.Add(child);
                await OrganizationTree(child, child.Children,allOrgs);
            });
        }

        private async Task Organization2OUTree(TreeViewModel current, List<TreeViewModel> childs,
                                               List<Organization> allOrgs,List<OU> ous)
        {
            var orgs = allOrgs.FindAll(o=>o.ParentId==current.Id);
            orgs.ForEach(async (org) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = org.Id,
                    ParentId = org.ParentId,
                    Name = org.OrgName,
                    Type = "org"
                };
                var childOUs = ous.FindAll(ou=>ou.OrganizationId==org.Id);
                childOUs.ForEach(cw =>
                {
                    var ouChild = new TreeViewModel
                    {
                        Id = cw.Id,
                        ParentId = child.Id,
                        Name = cw.OUName,
                        Type = "ou"
                    };
                    child.Children.Add(ouChild);
                });
                childs.Add(child);
                await Organization2OUTree(child, child.Children,allOrgs,ous);
            });
        }

        private async Task Organization2WarehouseTree(TreeViewModel current, List<TreeViewModel> childs,
            List<Organization> allOrgs,List<OU> ous,List<Warehouse> warehouses)
        {
            var orgs = allOrgs.FindAll(o=>o.ParentId==current.Id);
            orgs.ForEach(async (org) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = org.Id,
                    ParentId = org.ParentId,
                    Name = org.OrgName,
                    Type = "org"
                };

                var childOUs = ous.FindAll(ou=>ou.OrganizationId==org.Id);
                childOUs.ForEach(async(ou) =>
                {
                    var ouChild = new TreeViewModel
                    {
                        Id = ou.Id,
                        ParentId = child.Id,
                        Name = ou.OUName,
                        Type = "ou"
                    };
                    var childWarehouses = warehouses.FindAll(w=>w.OUId==ou.Id);
                    childWarehouses.ForEach(wh =>
                    {
                        var wareHouseChild = new TreeViewModel
                        {
                            Id = wh.Id,
                            ParentId = ouChild.Id,
                            Name = wh.WhName,
                            Type = "warehouse"
                        };
                        ouChild.Children.Add(wareHouseChild);
                    });
                    childs.Add(child);
                    child.Children.Add(ouChild);
                });
                childs.Add(child);
                await Organization2WarehouseTree(child, child.Children,allOrgs,ous,warehouses);
            });
        }

        private async Task Organization2AreaTree(TreeViewModel current, List<TreeViewModel> childs,
            List<Organization> allOrgs,List<OU> ous,List<Warehouse> warehouses,List<ReservoirArea> areas)
        {
            var orgs = allOrgs.FindAll(o=>o.ParentId==current.Id);
            orgs.ForEach(async (org) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = org.Id,
                    ParentId = org.ParentId,
                    Name = org.OrgName,
                    Type = "org"
                };
                
                var childOUs = ous.FindAll(ou=>ou.OrganizationId==org.Id);
                childOUs.ForEach(async(ou) =>
                {
                    var ouChild = new TreeViewModel
                    {
                        Id = ou.Id,
                        ParentId = child.Id,
                        Name = ou.OUName,
                        Type = "ou"
                    };
                    var childWarehouses = warehouses.FindAll(w=>w.OUId==ou.Id);
                    childWarehouses.ForEach(async (wh) =>
                    {
                        var wareHouseChild = new TreeViewModel
                        {
                            Id = wh.Id,
                            ParentId = ouChild.Id,
                            Name = wh.WhName,
                            Type = "warehouse"
                        };
                       
                        var childAreas = areas.FindAll(area=>area.WarehouseId==wh.Id);
                        childAreas.ForEach(async (ca) =>
                        {
                            var areaChild = new TreeViewModel
                            {
                                Id = ca.Id,
                                ParentId = wh.Id,
                                Name = ca.AreaName,
                                Type = "area"
                            };
                            wareHouseChild.Children.Add(areaChild);
                        });
                        ouChild.Children.Add(wareHouseChild);
                    });
                    child.Children.Add(ouChild);

                });
                childs.Add(child);
                await Organization2AreaTree(child, child.Children,allOrgs,ous,warehouses,areas);
            });
        }

        public async Task<ResponseResultViewModel> GetOrganizations(int? pageIndex, int? itemsPage,
            int? id, int? pid, string orgName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Organization> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new OrganizationPaginatedSpecification(pageIndex.Value,
                        itemsPage.Value,id, pid, orgName);
                }
                else
                {
                    baseSpecification = new OrganizationSpecification(id, pid, orgName);
                }
              
                var result =  await this._organizationRepository.ListAsync(baseSpecification);
                List<OrganizationViewModel> organizationViewModels = new List<OrganizationViewModel>();
                result.ForEach(r =>
                {
                    OrganizationViewModel organizationViewModel = new OrganizationViewModel
                    {
                        OrgName = r.OrgName,
                        Address = r.Address,
                        CreateTime = r.CreateTime.ToString(),
                        Id = r.Id,
                        Code = r.Code
                    };
                    organizationViewModels.Add(organizationViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._organizationRepository.CountAsync(new OrganizationSpecification(id, pid, orgName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = organizationViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = organizationViewModels;
                }
                
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }


        public async Task<ResponseResultViewModel> AddOrg(OrganizationViewModel org)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                Organization organization = new Organization
                {
                    Code = org.Code,
                    OrgName = org.OrgName,
                    Address = org.Address,
                    CreateTime = DateTime.Now,
                    ParentId = org.ParentId
                };
                await this._organizationService.AddOrg(organization);
                response.Data = org.Id;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> UpdateOrg(OrganizationViewModel org)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._organizationService.UpdateOrg(org.Id, org.OrgName, org.Code);
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
