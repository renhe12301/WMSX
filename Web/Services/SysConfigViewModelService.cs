using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.SysManager;

namespace Web.Services
{
    public class SysConfigViewModelService : ISysConfigViewModelService
    {

        private readonly ISysConfigService _sysConfigService;

        public SysConfigViewModelService(ISysConfigService sysConfigService) 
        {
            this._sysConfigService = sysConfigService;
        }

        public async Task<ResponseResultViewModel> UpdateConfig(SysConfigViewModel sysConfigViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
               await this._sysConfigService.UpdateConfig(sysConfigViewModel.KName, sysConfigViewModel.KVal);
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
