﻿using System;
namespace Web.ViewModels.OrderManager
{
    public class OrderRowViewModel
    {
        public int? Id { get; set; }

        public int OrderId { get; set; }
        public string RowNumber { get; set; }
        
        public int? ReservoirAreaId { get; set; }
        public string ReservoirAreaName { get; set; }

        public string OwnerType { get; set; }

        public string OwnerTypeName { get; set; }

        public int MaterialDicId { get; set; }
        public string MaterialDicName { get; set; }
        
        public string CreateTime { get; set; }
        public string FinishTime { get; set; }
        public double PreCount { get; set; }
        
        public double RealityCount { get; set; }
        public double Sorting { get; set; }

        public int BadCount { get; set; }
        public int Progress { get; set; }
      
        public string Memo { get; set; }

        public int? Status { get; set; }

        public string StatusStr { get; set; }

        public double? Price { get; set; }

        public double? Amount { get; set; }

        public int EBSTaskId { get; set; }

        public string EBSTaskName { get; set; }
        public int EBSProjectId { get; set; }
        public string ProjectName { get; set; }

        public string TrayCode { get; set; }

        public string Tag { get; set; }

        public int? OUId { get; set; }
        public int? WarehouseId { get; set; }
        public int? SupplierId { get; set; }
        public int? SupplierSiteId { get; set; }

        public string UseFor { get; set; }

        public int? RelatedId { get; set; }

        public int? SourceId { get; set; }

        public int? OrderTypeId { get; set; }

        public string Currency { get; set; }

        public string BusinessTypeCode { get; set; }

        public string SourceOrderType { get; set; }

        public double? Expend { get; set; }

        public string ExpenditureType { get; set; }

        public int EmployeeId { get; set; }

        public string ApplyUserCode { get; set; }

        public string ApproveUserCode { get; set; }
    }
}
