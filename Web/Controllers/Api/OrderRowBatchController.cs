using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;

namespace Web.Controllers.Api
{
    [EnableCors("AllowCORS")]
    public class OrderRowBatchController:BaseApiController
    {
        
        private readonly IOrderRowBatchViewModelService _orderRowBatchViewModelService;
        public OrderRowBatchController(IOrderRowBatchViewModelService orderRowBatchViewModelService)
        {
            this._orderRowBatchViewModelService = orderRowBatchViewModelService;
        }

        /// <summary>
        /// 获得出库订单批次数量
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="itemsPage"></param>
        /// <param name="id"></param>
        /// <param name="orderId"></param>
        /// <param name="orderRowId"></param>
        /// <param name="areaId"></param>
        /// <param name="isRead"></param>
        /// <param name="type"></param>
        /// <param name="isSync"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderRowBatchs(int? pageIndex, int? itemsPage, int? id, int? orderId,
            int? orderRowId, int? areaId, int? isRead, int? type, int? isSync, string status)
        {
            var response = await this._orderRowBatchViewModelService.GetOrderRowBatchs(pageIndex, itemsPage, id,
                orderId,orderRowId, areaId, isRead, type, isSync, status);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}