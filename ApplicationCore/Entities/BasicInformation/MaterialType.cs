using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 物料类型实体类
    /// </summary>
    public class MaterialType : BaseEntity
    {
        public string TypeName { get; set; }
        public int ParentId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
