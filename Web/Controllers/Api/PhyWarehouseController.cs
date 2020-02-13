using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;

namespace Web.Controllers.Api
{
    [EnableCors("AllowCORS")]
    public class PhyWarehouseController:BaseApiController
    {
        private readonly IPhyWarehouseViewModelService _phyWarehouseViewModelService;

        public PhyWarehouseController(IPhyWarehouseViewModelService phyWarehouseViewModelService)
        {
            this._phyWarehouseViewModelService = phyWarehouseViewModelService;
        }

        /// <summary>
        /// 获取物理仓库信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页大小</param>
        /// <param name="id">仓库编号</param>
        /// <param name="phyName">仓库名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPhyWarehouses(int? pageIndex, int? itemsPage, int? id,string phyName)
        {
            var response = await this._phyWarehouseViewModelService.GetPhyWarehouses(pageIndex, itemsPage, id, phyName);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 获取物理仓库树信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPhyWarehouseTrees()
        {
            var response = await this._phyWarehouseViewModelService.GetPhyWarehouseTrees();
            return Content(JsonConvert.SerializeObject(response));
        }
        
    }
}