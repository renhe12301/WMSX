using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Entities.BasicInformation
{
    public class EmployeeOrg : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int OrganizationId { get; set; }

        public Employee Employee { get; set; }
        public Organization Organization { get; set; }

    }
}
