using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Entities.OrderManager
{
    /// <summary>
    /// 出入库拆分订单
    /// </summary>
    public class SubOrder:BaseEntity
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
        public int? Status { get; set; }

        /// <summary>
        /// 物理仓库编号
        /// </summary>
        
        public int? PhyWarehouseId { get; set; }
        
        public PhyWarehouse PhyWarehouse { get; set; }
        
        /// <summary>
        /// 订单是否作废
        /// </summary>
        public int IsScrap { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public int IsRead { get; set; }

        /// <summary>
        /// 关联拆分订单行
        /// </summary>
        [NotMapped]
        public List<SubOrderRow> SubOrderRow { get; set; }

        [NotMapped]
        public string Tag { get; set; }
    }
}
