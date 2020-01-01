using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 托盘类型实体类
    /// </summary>
    public class TrayType:BaseEntity
    {
        public string TypeName { get; set; }
        public int ParentId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
