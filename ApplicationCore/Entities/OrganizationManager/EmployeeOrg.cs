using System;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Entities.OrganizationManager
{
    public class EmployeeOrg : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int OrgId { get; set; }
        public int WarehouseId { get; set; }

        public Employee Employee { get; set; }
        public Organization Organization { get; set; }
        public Warehouse Warehouse { get; set; }
        
    }
}
