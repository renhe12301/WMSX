using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.AuthorityManager;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using ApplicationCore.Misc;
using System.Collections.Generic;
using System.Linq;

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

        public async Task AddEmployee(Employee employee,bool unique=false)
        {
            Guard.Against.Null(employee, nameof(employee));
            Guard.Against.Zero(employee.Id, nameof(employee.Id));
            Guard.Against.NullOrEmpty(employee.UserCode,nameof(employee.UserCode));
            Guard.Against.NullOrEmpty(employee.UserName,nameof(employee.UserName));
            if (unique)
            {
                EmployeeSpecification employeeSpec = new EmployeeSpecification(employee.Id, null, null);
                var employees = await this._employeeRepository.ListAsync(employeeSpec);
                if (employees.Count == 0)
                    await this._employeeRepository.AddAsync(employee);
            }
            else
            {
                await this._employeeRepository.AddAsync(employee);
            }
        }

        public async Task UpdateEmployee(Employee employee)
        {
            Guard.Against.Null(employee, nameof(employee));
            await  this._employeeRepository.UpdateAsync(employee);
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
        
        
        public async Task AddEmployee(List<Employee> employees,bool unique=false)
        {
            Guard.Against.Null(employees, nameof(employees));
            Guard.Against.NullOrEmpty(employees,nameof(employees));
            if (unique)
            {
                List<Employee> adds=new List<Employee>();
                employees.ForEach(async(em) =>
                {
                    EmployeeSpecification employeeSpec=new EmployeeSpecification(em.Id,null,null);
                    var findEmployees = await this._employeeRepository.ListAsync(employeeSpec);
                    if(findEmployees.Count>0)
                        adds.Add(employees.First());
                });
                if (adds.Count > 0)
                    await this._employeeRepository.AddAsync(adds);
            }
            else
            {
                await this._employeeRepository.AddAsync(employees);
            }
        }

        public async Task UpdateEmployee(List<Employee> employees)
        {
            Guard.Against.Null(employees, nameof(employees));
            await this._employeeRepository.UpdateAsync(employees);
        }
        
    }
}
