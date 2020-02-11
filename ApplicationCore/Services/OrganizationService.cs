using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IAsyncRepository<Organization> _organizationRepository;

        public OrganizationService(IAsyncRepository<Organization> organizationRepository)
        {
            this._organizationRepository = organizationRepository;
        }

        public async Task AddOrg(Organization org)
        {
            Guard.Against.Null(org, nameof(org));
            Guard.Against.NullOrEmpty(org.OrgCode, nameof(org.OrgCode));
            Guard.Against.NullOrEmpty(org.OrgName, nameof(org.OrgName));
            OrganizationSpecification organizationSpec = new OrganizationSpecification(null, org.OrgCode, null, null);
            List<Organization> orgs = await this._organizationRepository.ListAsync(organizationSpec);
            if (orgs.Count == 0)
                await this._organizationRepository.AddAsync(org);
        }

        public async Task UpdateOrg(Organization srcOrg)
        {
            Guard.Against.Null(srcOrg, nameof(srcOrg));
            Guard.Against.Zero(srcOrg.Id, nameof(srcOrg.Id));
            Guard.Against.NullOrEmpty(srcOrg.OrgName, nameof(srcOrg.OrgName));
            Guard.Against.NullOrEmpty(srcOrg.OrgCode, nameof(srcOrg.OrgCode));
            var orgSpec = new OrganizationSpecification(srcOrg.Id, null, null, null);
            var orgs = await this._organizationRepository.ListAsync(orgSpec);
            if (orgs.Count > 0)
            {
                var targetOrg = orgs[0];
                targetOrg.OrgName = srcOrg.OrgName;
                targetOrg.OrgCode = srcOrg.OrgCode;
                targetOrg.CreateTime = srcOrg.CreateTime;
                targetOrg.EndTime = srcOrg.EndTime;
                targetOrg.OUId = srcOrg.OUId;
                await this._organizationRepository.UpdateAsync(targetOrg);
            }
        }
    }
}
