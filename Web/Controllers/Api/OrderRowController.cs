using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;

namespace Web.Controllers.Api
{
    [EnableCors("AllowCORS")]
    public class OrderRowController: BaseApiController
    {
        private readonly IOrderRowViewModelService _orderRowViewModelService;

        public OrderRowController(IOrderRowViewModelService orderRowViewModelService)
        {
            this._orderRowViewModelService = orderRowViewModelService;
        }

        /// <summary>
        /// 获取订单行信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">一页显示条数</param>
        /// <param name="id">行编号</param>
        /// <param name="orderId">订单编号</param>
        /// <param name="status">订单行状态</param>
        /// <param name="sCreateTime">订单行创建时间范围</param>
        /// <param name="eCreateTime">订单行创建时间范围</param>
        /// <param name="sFinishTime">订单行结束开始时间范围</param>
        /// <param name="eFinishTime">订单行结束时间范围</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderRows(int? pageIndex, int? itemsPage, int? id, int? orderId, string status,
            string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime)
        {
            var response = await this._orderRowViewModelService.GetOrderRows(pageIndex, itemsPage, id, orderId, status,
                sCreateTime, eCreateTime, sFinishTime, eFinishTime);
            return Content(JsonConvert.SerializeObject(response));
        }

    }
}