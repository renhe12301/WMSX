using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.SysManager;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class SysConfigService:ISysConfigService
    {
        private readonly IAsyncRepository<SysConfig> _sysConfigRepository;

        public SysConfigService(IAsyncRepository<SysConfig> sysConfigRepository)
        {
            this._sysConfigRepository = sysConfigRepository;
        }

        public async Task UpdateConfig(string key, string val)
        {
            Guard.Against.NullOrEmpty(key, nameof(key));
            Guard.Against.NullOrEmpty(val, nameof(val));
            SysConfigSpecification configSpec = new SysConfigSpecification(null, key);
            var configs = await this._sysConfigRepository.ListAsync(configSpec);
            Guard.Against.Zero(configs.Count, nameof(configs.Count));
            var config = configs[0];
            config.KVal = val;
            await this._sysConfigRepository.UpdateAsync(config);
        }
    }
}
