using System;
namespace ApplicationCore.Entities.BasicInformation
{
    public class TrayDicType:BaseEntity
    {
       public int TrayDicId { get; set; }
       public int TrayTypeId { get; set; }
       public TrayDic TrayDic { get; set; }
       public TrayType TrayType { get; set; }
    }
}
