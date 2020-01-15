using System;
namespace Web.ViewModels.OrganizationManager
{ 
    public class OUViewModel
    {
       public int Id { get; set; }
       public string OUName { get; set; }
       public string OUCode { get; set; }
       public int OrganizationId { get; set; }
       public string OrgName { get; set; }
    }
}
