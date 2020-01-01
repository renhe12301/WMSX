using System;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ISysConfigService
    {
        Task UpdateConfig(string key,string val);
    }
}
