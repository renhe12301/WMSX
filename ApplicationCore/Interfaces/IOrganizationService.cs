using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IOrganizationService
    {
        Task AddOrg(Organization org,bool unique=false);
        Task UpdateOrg(Organization org);
        
        Task AddOrg(List<Organization> orgs,bool unique=false);
        Task UpdateOrg(List<Organization> orgs);
        
    }
}
