using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;

namespace ApplicationCore.Interfaces
{
    public interface IEmployeeService
    {
        Task AddEmployee(Employee employee);
        Task UpdateEmployee(Employee employee);
        Task Logout(List<int> userIds);
        Task Enable(List<int> userIds);
        Task AssignRole(int employeeId,List<int> roleIds);

    }
}
