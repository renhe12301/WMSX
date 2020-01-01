using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployees(int? pageIndex,int? itemsPage,int? employeeId,string userName)
        {
            var response  = await this._employeeViewModelService.GetEmployees(pageIndex,itemsPage,employeeId, userName);
            return Ok(response);
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
            return Ok(response);
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
            return Ok(response);
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
            return Ok(response);
        }

        /// <summary>
        /// 分配员工角色
        /// </summary>
        /// <param name="employeeRoleViewModel">员工角色实体</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignRole(EmployeeRoleViewModel employeeRoleViewModel)
        {
            var response = await this._employeeViewModelService.AssignRole(employeeRoleViewModel);
            return Ok(response);
        }

    }
}
