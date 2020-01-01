using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 托盘字典实体类
    /// </summary>
    public class TrayDic : BaseEntity
    {
        public string TrayName { get; set; }
        public string TrayCode { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
        public int FullLoadRange { get; set; }
    }
}
