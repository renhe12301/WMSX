using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ApplicationCore.Entities.BasicInformation;

namespace Web.ViewModels.OrderManager
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
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
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int OUId { get; set; }
        public string OUName { get; set; }
        public int EBSProjectId { get; set; }
        public string ProjectName { get; set; }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int SupplierSiteId { get; set; }

        public string SupplierSiteName { get; set; }
        public string Currency { get; set; }
        public double TotalAmount { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public List<OrderRowViewModel> OrderRows;
    }
}
