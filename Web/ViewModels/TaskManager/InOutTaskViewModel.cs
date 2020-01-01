using System;
using System.Collections.Generic;

namespace Web.ViewModels.TaskManager
{
    public class InOutTaskViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int OrderId { get; set; }
        public int OrderRowId { get; set; }
        public int OrderTypeId { get; set; }
        public string OrderType { get; set; }
        public int WarehouseTrayId { get; set; }
        public string WarehouseTrayCode { get; set; }
        public string MaterialCode { get; set; }
        public int MaterialCount { get; set; }
        public string SrcId { get; set; }
        public string TargetId { get; set; }
        public string StatusStr { get; set; }
        public int Status { get; set; }
        public string StepStr { get; set; }
        public int Step { get; set; }
        public int Progress { get; set; }
        public int Feedback { get; set; }
        public string CreateTime { get; set; }
        public string FinishTime { get; set; }
        public int IsRead { get; set; }
        public string Memo { get; set; }
        public int? WarehouseId { get; set; }
        public string LocationCode { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; }
        public List<string> TrayCodes = new List<string>();
    }
}
