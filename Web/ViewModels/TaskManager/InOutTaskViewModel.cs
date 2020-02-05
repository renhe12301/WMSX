using System;
using System.Collections.Generic;
using ApplicationCore.Entities.StockManager;
using Web.ViewModels.StockManager;
namespace Web.ViewModels.TaskManager
{
    public class InOutTaskViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int? OrderId { get; set; }
        public int? OrderRowId { get; set; }
        public string TrayCode { get; set; }
        public string SrcId { get; set; }
        public string TargetId { get; set; }
        public int Status { get; set; }
        public int Step { get; set; }
        public string StatusStr { get; set; }
        public string StepStr { get; set; }
        public string Type { get; set; }
        public int Progress { get; set; }
        public int Feedback { get; set; }
        public string CreateTime { get; set; }
        public string FinishTime { get; set; }
        public int IsRead { get; set; }
        public string Memo { get; set; }
        public int? WarehouseId { get; set; }
        public string LocationCode { get; set; }
        public int? VehicleId { get; set; }
        public int? OUId { get; set; }
        public string OUName { get; set; }
        public int? OrganizationId { get; set; }
        public string OrgName { get; set; }
        public string WarehouseName { get; set; }
      
        public string ReservoirAreaName { get; set;  }

        public List<WarehouseTray> WarehouseTrays { get; set; }

    }
}
