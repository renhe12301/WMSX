using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using System.Dynamic;

namespace Web.Services
{
    public class OrganizationViewModelService : IOrganizationViewModelService
    {
        private readonly IOrganizationService _organizationService;
        private readonly IAsyncRepository<Organization> _organizationRepository;

        public OrganizationViewModelService(IOrganizationService organizationService,
                                            IAsyncRepository<Organization> organizationRepository)
        {
            this._organizationService = organizationService;
            this._organizationRepository = organizationRepository;
        }
        
        public async Task<ResponseResultViewModel> GetOrganizations(int? pageIndex, int? itemsPage,
            int? id, int? ouId, string orgName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Organization> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new OrganizationPaginatedSpecification(pageIndex.Value,
                        itemsPage.Value,id, ouId, orgName);
                }
                else
                {
                    baseSpecification = new OrganizationSpecification(id, null,ouId, orgName);
                }
              
                var result =  await this._organizationRepository.ListAsync(baseSpecification);
                List<OrganizationViewModel> organizationViewModels = new List<OrganizationViewModel>();
                result.ForEach(r =>
                {
                    OrganizationViewModel organizationViewModel = new OrganizationViewModel
                    {
                        OrgName = r.OrgName,
                        OrgCode = r.OrgCode,
                        CreateTime = r.CreateTime.ToString(),
                        Id = r.Id,
                        OUName = r.OU?.OUName
                    };
                    organizationViewModels.Add(organizationViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._organizationRepository.CountAsync(new OrganizationSpecification(id, null,ouId, orgName));
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
        
    }
}
