using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Entities.BasicInformation;

namespace Web.ViewModels.OrderManager
{
    /// <summary>
    /// 出入库拆分订单
    /// </summary>
    public class SubOrderViewModel
    {

        public int Id { get; set; }

        /// <summary>
        /// 订单编码
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 订单类型编号
        /// </summary>
        public int OrderTypeId { get; set; }

        public string OrderType { get; set; }

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
        public string CreateTime { get; set; }

        /// <summary>
        /// 订单完成时间
        /// </summary>
        public string FinishTime { get; set; }

        /// <summary>
        /// 库组织编号
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// 关联库存组织
        /// </summary>
        public string WarehouseName { get; set; }
        
        /// <summary>
        /// 业务实体编号
        /// </summary>
        public int OUId { get; set; }

        /// <summary>
        /// 关联业务实体
        /// </summary>
        public OU OU { get; set; }

        public string OUName { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? SupplierId { get; set; }

        /// <summary>
        /// 关联供应商头
        /// </summary>
        public string SpplierName { get; set; }

        /// <summary>
        /// 供应商站点编号
        /// </summary>
        public int? SupplierSiteId { get; set; }
        
        /// <summary>
        /// 关联供应商地址
        /// </summary>
        public string SupplierSiteName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set;  }

        /// <summary>
        /// 总金额
        /// </summary>
        public double TotalAmount { get; set; }

        /// <summary>
        /// 业务类型编码
        /// </summary>
        public string BusinessTypeCode { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int? Status { get; set; }
        
        public string StatusStr { get; set; }

        /// <summary>
        /// 物理仓库编号
        /// </summary>
        
        public int? PhyWarehouseId { get; set; }
        
        public string PhyWarehouseName { get; set; }
        
        /// <summary>
        /// 订单是否作废
        /// </summary>
        public int IsScrap { get; set; }

        public string IsScrapStr { get; set; }

        /// <summary>
        /// 关联拆分订单行
        /// </summary>
        public List<SubOrderRowViewModel> SubOrderRows { get; set; }
        
        public string Tag { get; set; }
    }
}
