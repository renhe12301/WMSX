using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EmployeeSpecification:BaseSpecification<Employee>
    {
        public EmployeeSpecification(int? id,int? depId,string employeeName,string userCode)
            : base(b =>(!id.HasValue || b.Id == id) &&
                       (!depId.HasValue || b.OrganizationId == depId)&&
                       (userCode == null || b.UserCode== userCode)&&
                       (employeeName == null || b.UserName.Contains(employeeName)))
        {
           
        }
    }
}
