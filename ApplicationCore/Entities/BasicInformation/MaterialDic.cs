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
        public string Unit { get; set; }
        public string Img { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Memo { get; set; }
        public int? UpLimit { get; set; }
        public int? DownLimit { get; set; }
        
        public int? WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public string Brand { get; set; }
        public int? Status { get; set; }

        public int? IsStock { get; set; }

        public int? MaterialTypeId { get; set; }
        public MaterialType MaterialType { get; set; }
        
    }
}
