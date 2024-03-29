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
        ///  获得拆分订单数据
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">当前页</param>
        /// <param name="ids">编号</param>
        /// <param name="orderNumber">订单号</param>
        /// <param name="sourceId">集约化物资系统订单号</param>
        /// <param name="orderTypeIds">订单类型</param>
        /// <param name="status">状态</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="warehouseId">库存组织编号</param>
        /// <param name="pyId">物理仓库编号</param>
        /// <param name="supplierId">供应商编号</param>
        /// <param name="supplierName">供应商名称</param>
        /// <param name="supplierSiteId">供应商站编号</param>
        /// <param name="supplierSiteName">供应商站名称</param>
        /// <param name="sCreateTime">创建开始时间</param>
        /// <param name="eCreateTime">创建结束时间</param>
        /// <param name="sFinishTime">完成开始时间</param>
        /// <param name="eFinishTime">完成结束时间</param>
        /// <param name="isBack">是否可以拆分退库单</param>
        /// <param name="businessType">业务类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrders(int? pageIndex,int? itemsPage,
            string ids,string orderNumber,int? sourceId, string orderTypeIds,string businessType,
            string status,int? isBack,int? ouId,int? warehouseId,int? pyId,
            int? supplierId, string supplierName,
            int? supplierSiteId,string supplierSiteName,
            string sCreateTime, string eCreateTime,
            string sFinishTime, string eFinishTime)
        {
            var response = await this._subOrderViewModelService.GetOrders(pageIndex,itemsPage,ids,orderNumber,sourceId,
                orderTypeIds, businessType, status, isBack, ouId,warehouseId,pyId, supplierId,supplierName,
                supplierSiteId, supplierSiteName, sCreateTime,eCreateTime,sFinishTime,eFinishTime);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 获得拆分订单行数据
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">当前页</param>
        /// <param name="ids">订单行编号</param>
        /// <param name="sourceId">集约化物资系统订单编号</param>
        /// <param name="subOrderId">订单头编号</param>
        /// <param name="orderRowId">关联订单行编号</param>
        /// <param name="orderTypeIds">关联订单类型编号</param>
        /// <param name="ouId">业务实体编号</param>
        /// <param name="warehouseId">库存组织编号</param>
        /// <param name="reservoirAreaId">子库区编号</param>
        /// <param name="businessType">业务类型</param>
        /// <param name="ownerType">库区类型 CONSIGNMENT寄售库 ORDINARY一般库</param>
        /// <param name="pyId">物理仓库编号</param>
        /// <param name="supplierId">供应商编号</param>
        /// <param name="supplierName">供应商名称</param>
        /// <param name="supplierSiteId">供应商站点编号</param>
        /// <param name="supplierSiteName">供应商站点名称</param>
        /// <param name="status">状态</param>
        /// <param name="sCreateTime">创建开始时间</param>
        /// <param name="eCreateTime">创建结束时间</param>
        /// <param name="sFinishTime">完成开始时间</param>
        /// <param name="eFinishTime">完成结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderRows(int? pageIndex, int? itemsPage, string ids, int? subOrderId,
            int? orderRowId,int? sourceId,string orderTypeIds, int? ouId, int? warehouseId, int? reservoirAreaId, string businessType, string ownerType,
            int? pyId, int? supplierId, string supplierName,
            int? supplierSiteId, string supplierSiteName, string status, string sCreateTime, string eCreateTime,
            string sFinishTime,string eFinishTime)
        {
            var response = await this._subOrderViewModelService.GetOrderRows(pageIndex, itemsPage, ids, subOrderId,
                orderRowId,sourceId,orderTypeIds, ouId, warehouseId,reservoirAreaId, businessType, ownerType, pyId, supplierId, supplierName, supplierSiteId, supplierSiteName,
                status,sCreateTime, eCreateTime, sFinishTime, eFinishTime);
            return Content(JsonConvert.SerializeObject(response));
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
        /// 入库完成订单拆分退库订单
        /// </summary>
        /// <param name="orderViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateTKOrder([FromBody]SubOrderViewModel orderViewModel)
        {
            var response = await this._subOrderViewModelService.CreateTKOrder(orderViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 分拣订单行
        /// </summary>
        /// <param name="subOrderRowView">拆分订单实体</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SortingOrder([FromBody]SubOrderRowViewModel subOrderRowView)
        {
            var response = await this._subOrderViewModelService.SortingOrder(subOrderRowView.SubOrderId,subOrderRowView.Id,
                subOrderRowView.Sorting.Value,subOrderRowView.TrayCode,subOrderRowView.ReservoirAreaId.Value,
                subOrderRowView.PhyWarehouseId.GetValueOrDefault());
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 无订单分拣
        /// </summary>
        /// <param name="subOrderRowView">拆分订单实体</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SortingNoneOrder([FromBody]SubOrderRowViewModel subOrderRowView)
        {
            var response = await this._subOrderViewModelService.SortingNoneOrder(subOrderRowView.MaterialDicCode,
                subOrderRowView.Sorting.Value, subOrderRowView.TrayCode, subOrderRowView.ReservoirAreaId.Value,
                subOrderRowView.PhyWarehouseId.GetValueOrDefault());
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
        public async Task<IActionResult> ScrapOrderRow([FromBody]List<SubOrderRowViewModel> subOrderRowViewModels)
        {
            var response = await this._subOrderViewModelService.ScrapOrderRow(subOrderRowViewModels);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 确认出库单
        /// </summary>
        /// <param name="subOrderViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OutConfirm([FromBody]SubOrderViewModel subOrderViewModel)
        {
            var response = await this._subOrderViewModelService.OutConfirm(subOrderViewModel.Id,
                subOrderViewModel.PhyWarehouseId.GetValueOrDefault());
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}