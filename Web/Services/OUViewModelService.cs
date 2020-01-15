using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Specifications;
using Web.ViewModels.OrganizationManager;
using System.Collections.Generic;
using System.Dynamic;

namespace Web.Services
{
    public class OUViewModelService:IOUViewModelService
    {

        private IAsyncRepository<OU> _ouRepository;

        public OUViewModelService(IAsyncRepository<OU> ouRepository)
        {
            this._ouRepository = ouRepository;
        }

        public async Task<ResponseResultViewModel> GetOUs(int? pageIndex, int? itemsPage, int? id, string ouName, string ouCode,int? orgId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<OU> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new OUPaginatedSpecification(pageIndex.Value,
                        itemsPage.Value, id, ouName, ouCode,orgId);
                }
                else
                {
                    baseSpecification = new OUSpecification(id,ouName,ouCode,orgId);
                }

                var result = await this._ouRepository.ListAsync(baseSpecification);
                List<OUViewModel> ouViewModels = new List<OUViewModel>();
                result.ForEach(r =>
                {
                    OUViewModel ouViewModel = new OUViewModel
                    {
                        OUName = r.OUName,
                        OUCode = r.OUCode,
                        Id = r.Id,
                        OrgName=r.Organization.OrgName
                    };
                    ouViewModels.Add(ouViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._ouRepository.CountAsync(new OUSpecification(id, ouName, ouCode, orgId));
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
    }
}
