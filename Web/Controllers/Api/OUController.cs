﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Newtonsoft.Json;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 业务实体Api
    /// </summary>
    [EnableCors("AllowCORS")]
    public class OUController : BaseApiController
    {
        private readonly IOUViewModelService _ouViewModelService;
        public OUController(IOUViewModelService ouViewModelService)
        {
            this._ouViewModelService = ouViewModelService;
        }

        /// <summary>
        /// 获取业务实体信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">一页项数</param>
        /// <param name="id">ou id</param>
        /// <param name="ouName">ou 名称</param>
        /// <param name="ouCode">ou 编码</param>
        /// <param name="companyName">法人</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOUs(int? pageIndex, int? itemsPage, int? id, string ouName, string ouCode, string companyName)
        {
            var response = await this._ouViewModelService.GetOUs(pageIndex, itemsPage, id, ouName, ouCode, companyName);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 获取业务实体树
        /// </summary>
        /// <param name="rootId">业务实体编号</param>
        /// <param name="depthTag"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOUTrees(int rootId,string depthTag)
        {
            var response = await this._ouViewModelService.GetOUTrees(rootId,depthTag);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}
