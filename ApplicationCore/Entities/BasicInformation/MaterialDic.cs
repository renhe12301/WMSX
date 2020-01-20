using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 物料字典实体类
    /// </summary>
    public class MaterialDic:BaseEntity
    {
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string Spec { get; set; }
        public int? MaterialUnitId { get; set; }
        public string Img { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
        public int? UpLimit { get; set; }
        public int? DownLimit { get; set; }
        public MaterialUnit MaterialUnit { get; set; }
    }
}
