using System;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrganizationManager;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 组织架构操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class OrganizationController : BaseApiController
    {

        private IAsyncRepository<Organization> _organizationRepository;
        private IOrganizationViewModelService _organizationViewModelService;

        public OrganizationController(IOrganizationViewModelService organizationViewModelService,
                                      IAsyncRepository<Organization> organizationRepository)
        {
            this._organizationViewModelService = organizationViewModelService;
            this._organizationRepository = organizationRepository;
        }

        /// <summary>
        /// 获取组织架构信息
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="itemsPage">一页项数</param>
        /// <param name="id">组织架构编号</param>
        /// <param name="pid">组织架构父编号</param>
        /// <param name="orgName">组织架构名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrganizations(int? pageIndex,int? itemsPage, int? id, int? pid, string orgName)
        {
            ResponseResultViewModel response =  await this._organizationViewModelService.GetOrganizations(pageIndex, itemsPage, id, pid, orgName);
            return Content(JsonConvert.SerializeObject(response));
        }

        
        /// <summary>
        /// 获取组织架构树信息
        /// </summary>
        /// <param name="id">组织架构编号</param>
        /// <param name="depthTag">树深度标记(User-用户 Warehouse-仓库 ReservoirArea-库区)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrganizationTrees(int rootId, string depthTag)
        {
            ResponseResultViewModel response = await this._organizationViewModelService.GetOrganizationTrees(rootId, depthTag);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 添加组织架构
        /// </summary>
        /// <param name="org">组织架构实体</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddOrg(OrganizationViewModel org)
        {
            ResponseResultViewModel response = await this._organizationViewModelService.AddOrg(org);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 更新组织架构
        /// </summary>
        /// <param name="org">组织架构实体</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateOrg(OrganizationViewModel org)
        {
            ResponseResultViewModel response = await this._organizationViewModelService.UpdateOrg(org);
            return Content(JsonConvert.SerializeObject(response));
        }
    }
}
