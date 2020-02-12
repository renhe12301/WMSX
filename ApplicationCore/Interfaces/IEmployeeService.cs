using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IEmployeeService
    {
        Task AddEmployee(Employee employee,bool unique=false);
        Task UpdateEmployee(Employee employee);

        Task AddEmployee(List<Employee> employees,bool unique=false);
        Task UpdateEmployee(List<Employee> employees);

        Task Logout(List<int> userIds);
        Task Enable(List<int> userIds);
        Task AssignRole(int employeeId,List<int> roleIds);

    }
}
