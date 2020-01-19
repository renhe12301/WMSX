using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrganizationManager;
using Web.ViewModels;
using Web.ViewModels.OrganizationManager;

namespace Web.Interfaces
{
    public interface IEmployeeViewModelService
    {
        Task<ResponseResultViewModel> AddEmployee(EmployeeViewModel employViewModel);
        Task<ResponseResultViewModel> Logout(EmployeeViewModel employViewModel);
        Task<ResponseResultViewModel> Enable(EmployeeViewModel employViewModel);
        Task<ResponseResultViewModel> AssignRole(EmployeeViewModel employeeViewModel);
        Task<ResponseResultViewModel> GetRoles(int employeeId);
        Task<ResponseResultViewModel> GetEmployees(int? pageIndex, int? itemsPage,
                                                   int? orgId,int? employeeId,string employeeName);

    }
}
