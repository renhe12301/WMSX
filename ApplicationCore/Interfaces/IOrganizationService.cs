using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Interfaces
{
    public interface IOrganizationService
    {
        Task AddOrg(Organization org);
        Task UpdateOrg(int orgId,string orgName,string orgCode);
    }
}
