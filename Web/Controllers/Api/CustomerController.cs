using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels.BasicInformation;

namespace Web.Controllers.Api
{

    /// <summary>
    /// 供应商操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class CustomerController:BaseApiController
    {

        private readonly ICustomerViewModelService _customerViewModelService;

        public CustomerController(ICustomerViewModelService customerViewModelService)
        {
            this._customerViewModelService = customerViewModelService;
        }

        /// <summary>
        /// 获取供应商信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">当前页显示大小</param>
        /// <param name="id">客户编号</param>
        /// <param name="customerName">客户名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomers(int? pageIndex, int? itemsPage,
          int? id, string customerName)
        {
            var response = await this._customerViewModelService.GetCustomers(pageIndex, itemsPage,id, customerName);
            return Ok(response);
        }

        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="customerViewModel">客户实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddSupplier(CustomerViewModel customerViewModel)
        {
            var response = await this._customerViewModelService.AddCustomer(customerViewModel);
            return Ok(response);
        }


    }
}
