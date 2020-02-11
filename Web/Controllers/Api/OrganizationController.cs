using System;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
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

    }
}
