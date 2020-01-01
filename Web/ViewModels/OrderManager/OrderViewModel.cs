using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Web.ViewModels.OrderManager
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public string OrderNumber { get; set; }
        public int OrderId { get; set; }
        public int OrderTypeId { get; set; }
        public string OrderType { get; set; }
        public string ApplyUserCode { get; set; }
        public string ApproveUserCode { get; set; }
        public string ApplyTime { get; set; }
        public string ApproveTime { get; set; }
        public string CallingParty { get; set; }
        public int Progress { get; set; }
        public string Memo { get; set; }
        public string CreateTime { get; set; }
        public string FinishTime { get; set; }
        public List<OrderRowViewModel> OrderRows;
    }
}
