using System;
using System.Collections.Generic;
using ApplicationCore.Entities.StockManager;
using Microsoft.AspNetCore.Http;
using Web.ViewModels.StockManager;
namespace Web.ViewModels.TaskManager
{
    public class InOutTaskViewModel
    {
        public int Id { get; set; }
        public int? SubOrderId { get; set; }
        public int? SubOrderRowId { get; set; }
        public string TrayCode { get; set; }

        public string MaterialCode { get; set; }
        public string SrcId { get; set; }
        public string TargetId { get; set; }
        public int Status { get; set; }
        public int Step { get; set; }
        public string StatusStr { get; set; }
        public string StepStr { get; set; }
        public string TypeStr { get; set; }
        public int Type { get; set; }
        public int Progress { get; set; }
        public int Feedback { get; set; }
        public string CreateTime { get; set; }
        public string FinishTime { get; set; }
        public int IsRead { get; set; }

        public string IsReadStr { get; set; }
        public string Memo { get; set; }
        public int? WarehouseId { get; set; }
        public string LocationCode { get; set; }
        public int? VehicleId { get; set; }
        public int? OUId { get; set; }
        public double? MaterialCount { get; set; }
        public string OUName { get; set; }
        public string WarehouseName { get; set; }

        public int? WarehouseTrayId { get; set; }
        

        public int? ReservoirAreaId { get; set; }
        public string ReservoirAreaName { get; set;  }
        
        public int? PhyWarehouseId { get; set; }
        
        public string PhyName { get; set; }

        public List<WarehouseTray> WarehouseTrays { get; set; }

    }
}
