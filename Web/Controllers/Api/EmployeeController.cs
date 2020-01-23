using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Interfaces;
using Web.ViewModels.OrganizationManager;

namespace Web.Controllers.Api
{
    /// <summary>
    /// 员工信息操作API
    /// </summary>
    [EnableCors("AllowCORS")]
    public class EmployeeController:BaseApiController
    {
        private readonly IEmployeeViewModelService _employeeViewModelService;

        public EmployeeController(IEmployeeViewModelService employeeViewModelService)
        {
            this._employeeViewModelService = employeeViewModelService;
        }

        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        /// <param name="employeeId">员工编号</param>
        /// <param name="employeeName">用户名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployees(int? pageIndex,int? itemsPage,int? orgId,int? employeeId,string employeeName)
        {
            var response  = await this._employeeViewModelService.GetEmployees(pageIndex,itemsPage,orgId,employeeId, employeeName);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 获取员工对应的角色信息
        /// </summary>
        /// <param name="employeeId">员工编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoles(int employeeId)
        {
            var response  = await this._employeeViewModelService.GetRoles(employeeId);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 添加员工信息
        /// </summary>
        /// <param name="employViewModel">员工信息JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeViewModel employViewModel)
        {
            var response = await this._employeeViewModelService.AddEmployee(employViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }
        
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="employViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(EmployeeViewModel employViewModel)
        {
            var response = await this._employeeViewModelService.UpdateEmployee(employViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 启用员工
        /// </summary>
        /// <param name="employViewModel">员工信息JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(EmployeeViewModel employViewModel)
        {
            var response = await this._employeeViewModelService.Enable(employViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 注销员工
        /// </summary>
        /// <param name="employViewModel">员工信息JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout(EmployeeViewModel employViewModel)
        {
            var response = await this._employeeViewModelService.Logout(employViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// 分配员工角色
        /// </summary>
        /// <param name="employeeViewModel">员工角色实体</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignRole(EmployeeViewModel employeeViewModel)
        {
            var response = await this._employeeViewModelService.AssignRole(employeeViewModel);
            return Content(JsonConvert.SerializeObject(response));
        }

    }
}
