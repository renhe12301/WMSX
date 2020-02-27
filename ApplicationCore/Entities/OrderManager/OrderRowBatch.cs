using System;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Entities.OrderManager
{
    public class OrderRowBatch:BaseEntity
    {
        public int? OrderId { get; set; }
        public int? OrderRowId { get; set; }
        public int BatchCount { get; set; }
        public int ReservoirAreaId { get; set; }
        public int IsRead { get; set; }

        public int IsSync { get; set; }

        public int Type { get; set; }

        public int Status { get; set; }
        public DateTime? CreateTime { get; set; }
        public Order Order { get; set; }
        public OrderRow OrderRow { get; set; }
        public ReservoirArea ReservoirArea { get; set; }

    }
}