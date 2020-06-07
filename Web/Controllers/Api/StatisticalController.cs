using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;
namespace Web.Controllers.Api
{
    
    [EnableCors("AllowCORS")]
    public class StatisticalController:BaseApiController
    {

        private readonly IStatisticalViewModelService _statisticalViewModelService;
        
        public StatisticalController(IStatisticalViewModelService statisticalViewModelService)
        {
            this._statisticalViewModelService = statisticalViewModelService;
        }
        
        /// <summary>
        /// 物料库存统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> MaterialChart(int ouId)
        {
            var response = await this._statisticalViewModelService.MaterialChart(ouId);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 物料库存统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> MaterialSheet(int ouId)
        {
            var response = await this._statisticalViewModelService.MaterialSheet(ouId);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 入库记录统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InRecordChart(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.InRecordChart(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 入库记录统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InRecordSheet(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.InRecordSheet(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 出库记录统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OutRecordChart(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.OutRecordChart(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 出库记录统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OutRecordSheet(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.OutRecordSheet(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 前置入库订单统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InOrderChart(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.InOrderChart(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 前置入库订单统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InOrderSheet(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.InOrderSheet(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 前置出库订单统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OutOrderChart(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.OutOrderChart(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 前置出库订单统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OutOrderSheet(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.OutOrderSheet(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 后置入库订单统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InSubOrderChart(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.InSubOrderChart(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 后置入库订单统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InSubOrderSheet(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.InSubOrderSheet(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 后置出库订单统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OutSubOrderChart(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.OutSubOrderChart(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 后置出库订单统计
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OutSubOrderSheet(int ouId,int queryType)
        {
            var response = await this._statisticalViewModelService.OutSubOrderSheet(ouId,queryType);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}