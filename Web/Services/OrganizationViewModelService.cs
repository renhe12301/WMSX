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
        private readonly IAsyncRepository<EmployeeOrg> _employeeOrgRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;

        public OrganizationViewModelService(IOrganizationService organizationService,
                                            IAsyncRepository<Organization> organizationRepository,
                                            IAsyncRepository<EmployeeOrg> employeeOrgRepository,
                                            IAsyncRepository<Warehouse> warehouseRepository,
                                            IAsyncRepository<ReservoirArea> reservoirAreaRepository)
        {
            this._organizationService = organizationService;
            this._organizationRepository = organizationRepository;
            this._employeeOrgRepository = employeeOrgRepository;
            this._warehouseRepository = warehouseRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
        }

        public async Task<ResponseResultViewModel> GetOrganizationTrees(int rootId,string depthTag)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var orgSpec = new OrganizationSpecification(rootId, null, null);
                var orgs =  await this._organizationRepository.ListAsync(orgSpec);
                if (orgs.Count == 0) throw new Exception(string.Format("组织架构编号{0}不存在", rootId));
                var org = orgs[0];
                TreeViewModel current = new TreeViewModel
                {
                    Id=org.Id,
                    ParentId=org.ParentId,
                    Name=org.OrgName,
                    Type="org"
                };

                if (depthTag == "org")
                {
                    await OrganizationTree(current, current.Children);
                }
                else if (depthTag == "warehouse")
                {
                    var wareHouseSpec = new WarehouseSpecification(null,null, current.Id, null);
                    var childWarehouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
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
                    await Organization2WarehouseTree(current, current.Children);
                }
                else if (depthTag == "area")
                {
                    var wareHouseSpec = new WarehouseSpecification(null,null, current.Id, null);
                    var childWarehouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
                    childWarehouses.ForEach(async (cw) =>
                    {
                        var wareHouseChild = new TreeViewModel
                        {
                            Id = cw.Id,
                            ParentId = current.Id,
                            Name = cw.WhName,
                            Type = "warehouse"
                        };
                        var areaSpec = new ReservoirAreaSpecification(null,null, cw.Id,null, null);
                        var childAreas = await this._reservoirAreaRepository.ListAsync(areaSpec);
                        childAreas.ForEach(async(ca) =>
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
                    await Organization2AreaTree(current, current.Children);
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

        private async Task OrganizationTree(TreeViewModel current, List<TreeViewModel> childs)
        {
            var orgSpec = new OrganizationSpecification(null, current.Id, null);
            var orgs = await this._organizationRepository.ListAsync(orgSpec);
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
                await OrganizationTree(child, child.Children);
            });
        }

       
        private async Task Organization2WarehouseTree(TreeViewModel current, List<TreeViewModel> childs)
        {
            var orgSpec = new OrganizationSpecification(null, current.ParentId, null);
            var orgs = await this._organizationRepository.ListAsync(orgSpec);
            orgs.ForEach(async (org) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = org.Id,
                    ParentId = org.ParentId,
                    Name = org.OrgName,
                    Type = "org"
                };
                var wareHouseSpec = new WarehouseSpecification(null,null, child.Id, null);
                var childWarehouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
                childWarehouses.ForEach(cw =>
                {
                    var wareHouseChild = new TreeViewModel
                    {
                        Id = cw.Id,
                        ParentId = child.Id,
                        Name = cw.WhName,
                        Type = "warehouse"
                    };
                    child.Children.Add(wareHouseChild);
                });
                childs.Add(child);
                await OrganizationTree(child, child.Children);
            });
        }

        private async Task Organization2AreaTree(TreeViewModel current, List<TreeViewModel> childs)
        {
            var orgSpec = new OrganizationSpecification(null, current.ParentId, null);
            var orgs = await this._organizationRepository.ListAsync(orgSpec);
            orgs.ForEach(async (org) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = org.Id,
                    ParentId = org.ParentId,
                    Name = org.OrgName,
                    Type = "org"
                };
                var wareHouseSpec = new WarehouseSpecification(null,null, child.Id, null);
                var childWarehouses = await this._warehouseRepository.ListAsync(wareHouseSpec);
                childWarehouses.ForEach(async(cw) =>
                {
                    var wareHouseChild = new TreeViewModel
                    {
                        Id = cw.Id,
                        ParentId = child.Id,
                        Name = cw.WhName,
                        Type = "warehouse"
                    };
                    var areaSpec = new ReservoirAreaSpecification(null,null,cw.Id,null,null);
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
                    child.Children.Add(wareHouseChild);
                });
                childs.Add(child);
                await OrganizationTree(child, child.Children);
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
