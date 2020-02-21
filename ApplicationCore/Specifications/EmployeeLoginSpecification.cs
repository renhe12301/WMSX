using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EmployeeLoginSpecification:BaseSpecification<Employee>
    {
        public EmployeeLoginSpecification(string loginName, string loginPwd)
            : base(b => b.LoginName==loginName&&b.LoginPwd==loginPwd)
        {
        }
    }
}