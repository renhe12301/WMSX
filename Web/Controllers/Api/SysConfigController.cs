using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels.SysManager;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 系统配置API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class SysConfigController: BaseApiController
    {
        private readonly ISysConfigViewModelService _sysConfigViewModelService;

        public SysConfigController(ISysConfigViewModelService sysConfigViewModelService) 
        {
            this._sysConfigViewModelService = sysConfigViewModelService;
        }

        /// <summary>
        /// 修改系统配置
        /// </summary>
        /// <param name="key">配置key</param>
        /// <param name="val">配置val</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateConfig([FromBody]SysConfigViewModel sysConfigViewModel)
        {
            var response = await this._sysConfigViewModelService.UpdateConfig(sysConfigViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}
