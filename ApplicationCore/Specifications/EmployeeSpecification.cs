using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EmployeeSpecification:BaseSpecification<Employee>
    {
        public EmployeeSpecification(int? id,string userName,string loginName)
            : base(b =>(!id.HasValue || b.Id == id) &&
                  (userName == null || b.UserName == userName)&&
                  (loginName == null || b.LoginName == loginName))
        {
           
        }
    }
}
