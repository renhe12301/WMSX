using System;
namespace ApplicationCore.Entities.OrganizationManager
{
    public class OU:BaseEntity
    {
       public string OUName { get; set; }
       public string OUCode { get; set; }
       public int OrganizationId { get; set; }

       public Organization Organization { get; set; }
    }
}
