using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class OrganizationService:IOrganizationService
    {
        private readonly IAsyncRepository<Organization> _organizationRepository;

        public OrganizationService(IAsyncRepository<Organization> organizationRepository)
        {
            this._organizationRepository = organizationRepository;
        }

        public async Task AddOrg(Organization org)
        {
            Guard.Against.Null(org, nameof(org));
            await this._organizationRepository.AddAsync(org);
        }

        public async Task UpdateOrg(int orgId, string orgName, string orgCode)
        {
            Guard.Against.Zero(orgId, nameof(orgId));
            Guard.Against.NullOrEmpty(orgName, nameof(orgName));
            Guard.Against.NullOrEmpty(orgCode, nameof(orgCode));
            var orgSpec = new OrganizationSpecification(orgId,null,null);
            var orgs = await this._organizationRepository.ListAsync(orgSpec);
            Guard.Against.NullOrEmpty(orgs, nameof(orgs));
            var org = orgs[0];
            org.OrgName = orgName;
            org.Code = orgCode;
            await this._organizationRepository.UpdateAsync(org);
        }
    }
}
