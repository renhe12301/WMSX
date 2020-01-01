using System;
namespace Web.ViewModels.OrderManager
{
    public class OrderType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
