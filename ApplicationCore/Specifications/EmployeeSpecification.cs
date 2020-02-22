using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EmployeeSpecification:BaseSpecification<Employee>
    {
        public EmployeeSpecification(int? id,int? orgId,string employeeName,string loginName)
            : base(b =>(!id.HasValue || b.Id == id) &&
                       (!orgId.HasValue || b.OrganizationId == orgId)&&
                       (loginName == null || b.UserName.Contains(loginName))&&
                       (employeeName == null || b.UserName.Contains(employeeName)))
        {
           
        }
    }
}
