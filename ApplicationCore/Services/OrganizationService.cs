using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task AddOrg(Organization org,bool unique=false)
        {
            Guard.Against.Null(org, nameof(org));
            Guard.Against.Zero(org.Id,nameof(org.Id));
            Guard.Against.NullOrEmpty(org.OrgCode, nameof(org.OrgCode));
            Guard.Against.NullOrEmpty(org.OrgName, nameof(org.OrgName));
            if (unique)
            { 
                OrganizationSpecification organizationSpec = new OrganizationSpecification(org.Id, null, null, null);
                List<Organization> orgs = await this._organizationRepository.ListAsync(organizationSpec);
                if (orgs.Count == 0)
                    await this._organizationRepository.AddAsync(org);
            }
            else
            {
                await this._organizationRepository.AddAsync(org);
            }
        }

        public async Task UpdateOrg(Organization srcOrg)
        {
            Guard.Against.Null(srcOrg, nameof(srcOrg));
            await this._organizationRepository.UpdateAsync(srcOrg);
        }

        public async Task AddOrg(List<Organization> orgs,bool unique=false)
        {
            Guard.Against.Null(orgs,nameof(orgs));
            if (unique)
            {
                List<Organization> adds=new List<Organization>();
                orgs.ForEach(async (o) =>
                {
                    OrganizationSpecification organizationSpec = new OrganizationSpecification(o.Id, null, null, null);
                    List<Organization> orgs = await this._organizationRepository.ListAsync(organizationSpec);
                    if (orgs.Count == 0)
                        adds.Add(o);
                });

                if (adds.Count > 0)
                    await this._organizationRepository.AddAsync(adds);
            }
            else
            {
                await this._organizationRepository.AddAsync(orgs);
            }
        }

        public async Task UpdateOrg(List<Organization> orgs)
        {
            Guard.Against.Null(orgs, nameof(orgs));
            await this._organizationRepository.UpdateAsync(orgs);
        }
    }
}
