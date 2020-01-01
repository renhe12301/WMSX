using System;
namespace ApplicationCore.Entities.BasicInformation
{
    public class MaterialDicType:BaseEntity
    {
       public int MaterialTypeId { get; set; }
       public int MaterialDicId { get; set; }

       public MaterialType MaterialType { get; set; }
       public MaterialDic MaterialDic { get; set; }
    }
}
