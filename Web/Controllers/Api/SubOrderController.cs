using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;
using Web.ViewModels.OrderManager;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 拆分子订单操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class SubOrderController:BaseApiController
    {
        private readonly ISubOrderViewModelService _subOrderViewModelService;

        public SubOrderController(ISubOrderViewModelService subOrderViewModelService)
        {
            this._subOrderViewModelService = subOrderViewModelService;
        }
        
        /// <summary>
        /// 拆分前置订单
        /// </summary>
        /// <param name="orderViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody]SubOrderViewModel orderViewModel)
        {
            var response = await this._subOrderViewModelService.CreateOrder(orderViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 拆分订单作废
        /// </summary>
        /// <param name="orderViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ScrapOrder([FromBody]SubOrderViewModel orderViewModel)
        {
            var response = await this._subOrderViewModelService.ScrapOrder(orderViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        
        /// <summary>
        /// 拆分订单行作废
        /// </summary>
        /// <param name="subOrderRowViewModels"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ScrapOrder([FromBody]List<SubOrderRowViewModel> subOrderRowViewModels)
        {
            var response = await this._subOrderViewModelService.ScrapOrderRow(subOrderRowViewModels);
            return Content(JsonConvert.SerializeObject(response));
        }
        
    }
}