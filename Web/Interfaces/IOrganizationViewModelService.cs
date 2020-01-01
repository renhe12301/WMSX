using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;

namespace Web.Interfaces
{
    public interface IOrganizationViewModelService
    {
        Task<ResponseResultViewModel> AddOrg(OrganizationViewModel org);

        Task<ResponseResultViewModel> UpdateOrg(OrganizationViewModel org);

        Task<ResponseResultViewModel> GetOrganizations(int? pageIndex,int? itemsPage,int? id, int? pid, string orgName);

        Task<ResponseResultViewModel> GetOrganizationTrees(int rootId,string depthTag);
    }
}
