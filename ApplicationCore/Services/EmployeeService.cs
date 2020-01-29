using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using ApplicationCore.Misc;
using System.Collections.Generic;

namespace ApplicationCore.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IAsyncRepository<Employee> _employeeRepository;
        private readonly IAsyncRepository<EmployeeRole> _employeeRoleRepository;

        public EmployeeService(IAsyncRepository<Employee> employeeRepository,
                               IAsyncRepository<EmployeeRole> employeeRoleRepository)
        {
            this._employeeRepository = employeeRepository;
            this._employeeRoleRepository = employeeRoleRepository;
        }

        public async Task AddEmployee(Employee employee)
        {
            Guard.Against.Null(employee, nameof(employee));
            await this._employeeRepository.AddAsync(employee);
        }

        public async Task UpdateEmployee(Employee employee)
        {
            Guard.Against.Null(employee, nameof(employee));
            Guard.Against.Zero(employee.Id, nameof(employee.Id));
            EmployeeSpecification employeeSpec=new EmployeeSpecification(null,null,employee.LoginName);
            var employees = await this._employeeRepository.ListAsync(employeeSpec);
            if (employees.Count > 0)
            {
                if (employees[0].Id != employee.Id)
                    throw new Exception(string.Format("登录名[{0}],已存在！", employee.LoginName));
            }
            employeeSpec=new EmployeeSpecification(employee.Id,null,null);
            employees = await this._employeeRepository.ListAsync(employeeSpec);
            Guard.Against.NullOrEmpty(employees,nameof(employees));
            var updEmployee = employees[0];
            updEmployee.Address = employee.Address;
            updEmployee.Email = employee.Email;
            updEmployee.Img = employee.Img;
            updEmployee.Memo = employee.Memo;
            updEmployee.Sex = employee.Sex;
            updEmployee.Telephone = employee.Telephone;
            updEmployee.LoginName = employee.LoginName;
            updEmployee.LoginPwd = employee.LoginPwd;
            await this._employeeRepository.UpdateAsync(updEmployee);
        }

        public async Task AssignRole(int employeeId,List<int> roleIds)
        {
            Guard.Against.Zero(employeeId, nameof(employeeId));

            EmployeeRoleSpecification erSpec = new EmployeeRoleSpecification(null, employeeId,null);
            var emRoles = await this._employeeRoleRepository.ListAsync(erSpec);
            if (emRoles.Count > 0)
                await this._employeeRoleRepository.DeleteAsync(emRoles);
            List<EmployeeRole> ers=new List<EmployeeRole>();
            roleIds.ForEach(async (roleId) =>
            {
                EmployeeRole employeeRole = new EmployeeRole();
                employeeRole.EmployeeId = employeeId;
                employeeRole.SysRoleId = roleId;
                ers.Add(employeeRole);
            });
            await this._employeeRoleRepository.AddAsync(ers);
        }

        public async Task Enable(List<int> userIds)
        {
            Guard.Against.NullOrEmpty(userIds, nameof(userIds));
            var users = await this._employeeRepository.ListAllAsync();
            List<Employee> updEmployees=new List<Employee>();
            users.ForEach(em =>
            {
                if (userIds.Contains(em.Id))
                {
                    em.Status = Convert.ToInt32(EMPLOYEE_STATUS.正常);
                    updEmployees.Add(em);
                }
            });
            if (updEmployees.Count > 0)
                await this._employeeRepository.UpdateAsync(updEmployees);
        }

        public async Task Logout(List<int> userIds)
        {
            Guard.Against.NullOrEmpty(userIds, nameof(userIds));
            var users = await this._employeeRepository.ListAllAsync();
            List<Employee> updEmployees=new List<Employee>();
            users.ForEach(em =>
            {
                if (userIds.Contains(em.Id))
                {
                    em.Status = Convert.ToInt32(EMPLOYEE_STATUS.禁用);
                    updEmployees.Add(em);
                }
            });
            if (updEmployees.Count > 0)
                await this._employeeRepository.UpdateAsync(updEmployees);
        }

        
    }
}
