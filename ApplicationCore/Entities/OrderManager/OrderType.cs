using System;
namespace ApplicationCore.Entities.OrderManager
{
    /// <summary>
    /// 订单类型实体类
    /// </summary>
    public class OrderType:BaseEntity
    {
        public string TypeName { get; set; }
        public DateTime CreateTime { get; set; }
        public int InOutTag { get; set; }
        public string Memo { get; set; }
    }
}
