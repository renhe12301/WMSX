using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        /// <param name="id">订单id</param>
        /// <param name="orderNumber">订单编号</param>
        /// <param name="sourceId">集约化物资系统订单编号</param>
        /// <param name="orderTypeId">订单类型id</param>
        /// <param name="status">订单状态</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="warehouseId">库存组织编号</param>
        ///  <param name="pyId">物理仓库编号</param>
        /// <param name="applyUserCode">申请人编码</param>
        /// <param name="approveUserCode">审批人编码</param>
        /// <param name="employeeId">经办人编号</param>
        /// <param name="employeeName">经办人名称</param>
        /// <param name="supplierId">供应商编号</param>
        /// <param name="supplierName">供应商名称</param>
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
        public async Task<IActionResult> GetOrders(int? pageIndex,int? itemsPage,
                     int?id,string orderNumber,int? sourceId, int? orderTypeId,
                     string status,int? ouId,int? warehouseId,int? pyId,
                     string applyUserCode, string approveUserCode,
                     int?employeeId,string employeeName,
                     int? supplierId, string supplierName,
                     string sApplyTime, string eApplyTime,
                     string sApproveTime, string eApproveTime,
                     string sCreateTime, string eCreateTime,
                     string sFinishTime, string eFinishTime)
        {
            var response = await this._orderViewModelService.GetOrders(pageIndex,itemsPage,id,orderNumber,sourceId,
                orderTypeId, status,ouId,warehouseId,pyId, applyUserCode, approveUserCode,employeeId,employeeName, 
                supplierId, supplierName, sApplyTime,
                eApplyTime, sApproveTime, eApproveTime,sCreateTime,eCreateTime,sFinishTime,eFinishTime);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        
        
        /// <summary>
        /// 获取订单行信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">一页显示条数</param>
        /// <param name="id">行编号</param>
        /// <param name="orderId">订单编号</param>
        ///  <param name="sourceId">集约化物资订单编号</param>
        /// <param name="status">订单行状态</param>
        /// <param name="sCreateTime">订单行创建时间范围</param>
        /// <param name="eCreateTime">订单行创建时间范围</param>
        /// <param name="sFinishTime">订单行结束开始时间范围</param>
        /// <param name="eFinishTime">订单行结束时间范围</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderRows(int? pageIndex, int? itemsPage, int? id, int? orderId,int? sourceId, string status,
            string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime)
        {
            var response = await this._orderViewModelService.GetOrderRows(pageIndex, itemsPage, id, orderId,sourceId, status,
                sCreateTime, eCreateTime, sFinishTime, eFinishTime);
            return Content(JsonConvert.SerializeObject(response));
        }

        
        /// <summary>
        /// 获取退库物料信息
        /// </summary>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="warehouseId">库存组织编号</param>
        /// <param name="areaId">子库存编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTKOrderMaterials(int ouId, int warehouseId, int? areaId)
        {
            var response = await this._orderViewModelService.GetTKOrderMaterials(ouId,warehouseId,areaId);
            return Content(JsonConvert.SerializeObject(response));
        }

    }

}
