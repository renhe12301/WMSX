using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels.OrderManager;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Controllers.Api
{
    /// <summary>
    /// 订单操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class OrderController : BaseApiController
    {
        private readonly IOrderViewModelService _orderViewModelService;

        public OrderController(IOrderViewModelService orderViewModelService)
        {
            this._orderViewModelService = orderViewModelService;
        }

        /// <summary>
        /// 查询订单信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">一页显示条数</param>
        /// <param name="includeDetail">是否包含订单物料明细</param>
        /// <param name="orderNumber">订单编号</param>
        /// <param name="orderTypeId">订单类型编号</param>
        /// <param name="progressRange">订单进度范围</param>
        /// <param name="applyUserCode">申请人编码</param>
        /// <param name="approveUserCode">审批人编码</param>
        /// <param name="sApplyTime">申请开始时间</param>
        /// <param name="eApplyTime">申请结束时间</param>
        /// <param name="sApproveTime">审批开始时间</param>
        /// <param name="eApproveTime">审批结束时间</param>
        /// <param name="sCreateTime">创建开始时间</param>
        /// <param name="eCreateTime">创建结束时间</param>
        /// <param name="sFinishTime">完成开始时间</param>
        /// <param name="eFinishTime">完成结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrders(int? pageIndex,int? itemsPage,int? includeDetail,
                     string orderNumber, int? orderTypeId,
                     string progressRange, string applyUserCode, string approveUserCode,
                     string sApplyTime, string eApplyTime,
                     string sApproveTime, string eApproveTime,
                     string sCreateTime, string eCreateTime,
                     string sFinishTime, string eFinishTime)
        {
            var response = await this._orderViewModelService.GetOrders(pageIndex,itemsPage,includeDetail,orderNumber,
                orderTypeId, progressRange, applyUserCode, approveUserCode, sApplyTime,
                eApplyTime, sApproveTime, eApproveTime,sCreateTime,eCreateTime,sFinishTime,eFinishTime);
            return Ok(response);
        }


        /// <summary>
        /// 订单入库分拣
        /// </summary>
        /// <param name="orderRowViewModel">入库订单行JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SortingOrder2Area(OrderRowViewModel orderRowViewModel)
        {
            var response = await this._orderViewModelService.SortingOrder2Area(orderRowViewModel);
            return Ok(response);
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="orderViewModel">订单JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderViewModel orderViewModel)
        {
            var response = await this._orderViewModelService.CreateOrder(orderViewModel);
            return Ok(response);
        }



    }

}
