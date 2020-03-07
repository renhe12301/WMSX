using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Entities.OrderManager
{
    /// <summary>
    /// 出入库订单
    /// </summary>
    public class Order:BaseEntity
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 订单类型编号
        /// </summary>
        public int OrderTypeId { get; set; }
        /// <summary>
        /// 申请人编码(员工唯一编码)
        /// </summary>
        public string ApplyUserCode { get; set; }
        /// <summary>
        /// 审批人编码(多个逗号隔开)
        /// </summary>
        public string ApproveUserCode { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? ApproveTime { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime? ApplyTime { get; set; }
        /// <summary>
        /// 接口调用方
        /// </summary>
        public string CallingParty { get; set; }
        /// <summary>
        /// 完成进度
        /// </summary>
        public int Progress { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 订单完成时间
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// 库组织编号
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// 关联库存组织
        /// </summary>
        public Warehouse Warehouse { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public int? EBSProjectId { get; set; }

        /// <summary>
        /// 关联EBS项目
        /// </summary>
        public EBSProject EBSProject { get; set; }

        /// <summary>
        /// 业务实体编号
        /// </summary>
        public int OUId { get; set; }

        /// <summary>
        /// 关联业务实体
        /// </summary>
        public OU OU { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? SupplierId { get; set; }

        /// <summary>
        /// 关联供应商头
        /// </summary>
        public Supplier Supplier { get; set; }

        /// <summary>
        /// 供应商站点编号
        /// </summary>
        public int? SupplierSiteId { get; set; }
        
        /// <summary>
        /// 关联供应商地址
        /// </summary>
        public SupplierSite SupplierSite { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set;  }

        /// <summary>
        /// 总金额
        /// </summary>
        public double TotalAmount { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// 业务类型编码
        /// </summary>
        public string BusinessTypeCode { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 第三方系统状态标记
        /// </summary>
        public int? StatusTag { get; set; }

        /// <summary>
        /// 经办人编号
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// 关联经办人-员工
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// 第三方系统编号
        /// </summary>
        public int? SourceId { get; set; }
        
        /// <summary>
        /// 物理仓库编号
        /// </summary>
        
        public int? PhyWarehouseId { get; set; }
        
        public PhyWarehouse PhyWarehouse { get; set; }

        /// <summary>
        /// 关联订单行
        /// </summary>
        [NotMapped]
        public List<OrderRow> OrderRow { get; set; }

        [NotMapped]
        public string Tag { get; set; }
    }
}
