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

        public Task UpdateEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public async Task AssignRole(int employeeId,List<int> roleIds)
        {
            Guard.Against.Zero(employeeId, nameof(employeeId));
            Guard.Against.NullOrEmpty(roleIds, nameof(roleIds));

            this._employeeRoleRepository.TransactionScope(async() =>
            {
                EmployeeRoleSpecification erSpec = new EmployeeRoleSpecification(null, employeeId,null);
                var emRoles = await this._employeeRoleRepository.ListAsync(erSpec);
                if (emRoles.Count > 0)
                    await this._employeeRoleRepository.DeleteAsync(emRoles);
                roleIds.ForEach(async (roleId) =>
                {
                    EmployeeRole employeeRole = new EmployeeRole();
                    employeeRole.EmployeeId = employeeId;
                    employeeRole.RoleId = roleId;
                    await this._employeeRoleRepository.AddAsync(employeeRole);
                });

            });
        }

        public async Task Enable(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            var emSpec = new EmployeeSpecification(id, null);
            var employees = await this._employeeRepository.ListAsync(emSpec);
            Guard.Against.NullOrEmpty(employees, nameof(employees));
            var employee = employees[0];
            employee.Status = Convert.ToInt32(EMPLOYEE_STATUS.正常);
        }

        public async Task Logout(int id)
        {
            var emSpec = new EmployeeSpecification(id, null);
            var employees = await this._employeeRepository.ListAsync(emSpec);
            Guard.Against.NullOrEmpty(employees, nameof(employees));
            var employee = employees[0];
            employee.Status = Convert.ToInt32(EMPLOYEE_STATUS.注销);
        }

        
    }
}
