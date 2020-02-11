using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities.BasicInformation
{
    /// <summary>
    /// 库区实体类
    /// </summary>
    public class ReservoirArea : BaseEntity
    {
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public int OUId { get; set; }
        public OU OU { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string Memo { get; set; }
       
    }
}
