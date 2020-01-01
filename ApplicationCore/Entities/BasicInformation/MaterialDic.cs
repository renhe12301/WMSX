using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 物料字典实体类
    /// </summary>
    public class MaterialDic:BaseEntity
    {
        public int SourceId { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string Spec { get; set; }
        public int? UnitId { get; set; }
        public string Img { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
        public MaterialUnit MaterialUnit { get; set; }
    }
}
