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
        /// 员工登录
        /// </summary>
        /// <param name="employViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(EmployeeViewModel employViewModel)
        {
            var response = await this._employeeViewModelService.Login(employViewModel);
            return Content(JsonConvert.SerializeObject(response));;
        }

        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="itemsPage">一页条数</param>
        ///  <param name="depId">部门编号</param>
        /// <param name="employeeId">员工编号</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="userCode">员工编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployees(int? pageIndex,int? itemsPage,int? depId,int? employeeId,
                                                      string employeeName,string userCode)
        {
            var response  = await this._employeeViewModelService.GetEmployees(pageIndex,itemsPage,depId,employeeId, 
                                                                              employeeName,userCode);
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
        /// 启用员工
        /// </summary>
        /// <param name="employViewModel">员工信息JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Enable(EmployeeViewModel employViewModel)
        {
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                employViewModel.Tag = cookie.loginName;
            }
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
