using System;
using ApplicationCore.Entities.BasicInformation;

namespace Web.ViewModels.OrderManager
{
    /// <summary>
    /// 出入库拆分订单行
    /// </summary>
    public class SubOrderRowViewModel
    {
        /// <summary>
        /// 订单行编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 关联订单Id
        /// </summary>
        public int SubOrderId { get; set; }

        /// <summary>
        /// 订单行编号
        /// </summary>
        public string RowNumber { get; set; }

        /// <summary>
        /// 关联分区编号
        /// </summary>
        public int? ReservoirAreaId { get; set; }

        /// <summary>
        /// 子库区名称
        /// </summary>
        public string ReservoirAreaName { get; set; }

        /// <summary>
        /// 业主编号
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// 业主类型
        /// </summary>
        public string OwnerType { get; set; }

        /// <summary>
        /// 业主类型名称
        /// </summary>
        public string OwnerTypeName { get; set; }

        /// <summary>
        /// 物料字典编号
        /// </summary>
        public int MaterialDicId { get; set; }
        
        /// <summary>
        /// 物料字典名称
        /// </summary>
        public string MaterialDicName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 订单完成时间
        /// </summary>
        public string FinishTime { get; set; }
        /// <summary>
        /// 订单物料数量
        /// </summary>
        public double PreCount { get; set; }
        /// <summary>
        /// 分拣数量
        /// </summary>
        public double? Sorting { get; set; }
        /// <summary>
        /// 实际数量
        /// </summary>
        public double? RealityCount { get; set; }

        /// <summary>
        /// 不合格数量
        /// </summary>
        public int? BadCount { get; set; }
        
        /// <summary>
        /// 完成进度
        /// </summary>
        public int? Progress { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 订单行状态
        /// </summary>
        public int? Status { get; set; }

        public string StatusStr { get; set; }

        /// <summary>
        /// 关联前置订单行编号
        /// </summary>
        public int? OrderRowId { get; set; }
        
        /// <summary>
        /// 不含税单价
        /// </summary>
        public double? Price { get; set; }
        
        /// <summary>
        /// 不含税金额
        /// </summary>
        public double? Amount { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public string UseFor { get; set; }
        
        /// <summary>
        /// 订单是否作废
        /// </summary>
        public int IsScrap { get; set; }
        
        /// <summary>
        /// 业务实体编号
        /// </summary>
        public int? OUId { get; set; }
        
        /// <summary>
        /// 库存组织编号
        /// </summary>
        public int? WarehouseId { get; set; }
        
        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? SupplierId { get; set; }
        
        /// <summary>
        /// 供应商站点编号
        /// </summary>
        public int? SupplierSiteId { get; set; }
        
        /// <summary>
        /// 是否作废显示名称
        /// </summary>
        public string IsScrapStr { get; set; }

        /// <summary>
        /// 前置订单行编号
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// 托盘码
        /// </summary>
        public string TrayCode { get; set; }

        public int Tag { get; set; }

        /// <summary>
        /// 订单头类型编号
        /// </summary>
        public int? OrderTypeId { get; set; }
        
        /// <summary>
        ///  物理仓库编号
        /// </summary>
        public int? PhyWarehouseId { get; set; }

        /// <summary>
        /// 支出类型
        /// </summary>
        public string ExpenditureType { get; set; }

        /// <summary>
        /// EBS 任务编号
        /// </summary>
        public int EBSTaskId { get; set; }

        /// <summary>
        /// EBS 任务名称
        /// </summary>
        public string EBSTaskName { get; set; }

        /// <summary>
        /// EBS 项目编号
        /// </summary>
        public int EBSProjectId { get; set; }

        /// <summary>
        /// EBS 项目名称
        /// </summary>
        public string EBSProjectName { get; set; }

    }
}
