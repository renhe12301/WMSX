using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;
using Web.ViewModels.BasicInformation;

namespace Web.Controllers.Api
{

    /// <summary>
    /// 供应商操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class SupplierController:BaseApiController
    {

        private readonly ISupplierViewModelService _supplierViewModelService;

        public SupplierController(ISupplierViewModelService supplierViewModelService)
        {
            this._supplierViewModelService = supplierViewModelService;
        }

        /// <summary>
        /// 获取供应商信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页显示大小</param>
        /// <param name="id">供应商编号</param>
        /// <param name="supplierName">供应商名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSuppliers(int? pageIndex, int? itemsPage,
          int? id, string supplierName)
        {
            var response = await this._supplierViewModelService.GetSuppliers(pageIndex, itemsPage,id, supplierName);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 获取供应商地址信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页显示大小</param>
        /// <param name="id">供应商地址编号</param>
        /// <param name="supplierName">供应商名称</param>
        /// <param name="supplierId">供应商编号</param>
        /// <param name="ouId">业务实体编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSupplierSites(int? pageIndex, int? itemsPage, int? id, string supplierName,
            int? supplierId,int? ouId)
        {
            var response = await this._supplierViewModelService.GetSupplierSites(pageIndex, itemsPage,id, supplierName,supplierId,ouId);
            return Content(JsonConvert.SerializeObject(response));
        }


    }
}
