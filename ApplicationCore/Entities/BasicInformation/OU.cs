using System;
namespace ApplicationCore.Entities.BasicInformation
{
    public class OU:BaseEntity
    {
       public string OUName { get; set; }
       public string OUCode { get; set; }

       public string CompanyName { get; set; }

       public string PlateCode { get; set; }

       public string PlateName { get; set; }
       public DateTime? CreateTime { get; set; }
       public DateTime? EndTime { get; set; }
    }
}
