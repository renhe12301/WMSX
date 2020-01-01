using System;
namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 客户实体类
    /// </summary>
    public class Customer:BaseEntity
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Telephone { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
