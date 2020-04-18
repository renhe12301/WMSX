using System;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Specifications
{
    public class OrderPaginatedSpecification : BaseSpecification<Order>
    {
        public OrderPaginatedSpecification(int skip, int take, int? id, string orderNumber, int? orderTypeId,
             List<int> status, int? ouId, int? warehouseId, int? pyId, string applyUserCode, string approveUserCode,
             int? employeeId, string employeeName, int? supplierId, string supplierName, string sApplyTime, string eApplyTime,
             string sApproveTime, string eApproveTime,
             string sCreateTime, string eCreateTime,
             string sFinishTime, string eFinishTime)
            : base(b => (!id.HasValue || b.Id == id) &&
                       (orderNumber == null || b.OrderNumber.Contains(orderNumber)) &&
                       (!orderTypeId.HasValue || b.OrderTypeId == orderTypeId) &&
                       (status == null || status.Contains(b.Status.GetValueOrDefault())) &&
                       (!ouId.HasValue || b.OUId == ouId) &&
                       (!warehouseId.HasValue || b.WarehouseId == warehouseId) &&
                       (!pyId.HasValue || b.PhyWarehouseId == pyId) &&
                       (applyUserCode == null || b.ApplyUserCode == applyUserCode) &&
                       (approveUserCode == null || b.ApproveUserCode == approveUserCode) &&
                       (!employeeId.HasValue || b.EmployeeId == employeeId) &&
                       (employeeName == null || b.Employee.UserName.Contains(employeeName)) &&
                       (!supplierId.HasValue || b.SupplierId == supplierId) &&
                       (supplierName == null || b.Supplier.SupplierName.Contains(supplierName)) &&
                       (sApplyTime == null || b.ApplyTime >= DateTime.Parse(sApplyTime)) &&
                       (eApplyTime == null || b.ApplyTime <= DateTime.Parse(eApplyTime)) &&
                       (sApproveTime == null || b.ApproveTime >= DateTime.Parse(sApproveTime)) &&
                       (eApproveTime == null || b.ApproveTime <= DateTime.Parse(eApproveTime)) &&
                       (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                       (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)) &&
                       (sFinishTime == null || b.FinishTime >= DateTime.Parse(sFinishTime)) &&
                       (eFinishTime == null || b.FinishTime <= DateTime.Parse(eFinishTime)))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.OrderType);
            AddInclude(b => b.OU);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.Employee);
            AddInclude(b => b.Supplier);
            AddInclude(b => b.SupplierSite);
            AddInclude(b => b.EBSProject);
            AddInclude(b => b.PhyWarehouse);
        }
    }
}
