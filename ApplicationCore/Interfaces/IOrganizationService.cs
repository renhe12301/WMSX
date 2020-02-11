using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IOrganizationService
    {
        Task AddOrg(Organization org);
        Task UpdateOrg(Organization org);
    }
}
