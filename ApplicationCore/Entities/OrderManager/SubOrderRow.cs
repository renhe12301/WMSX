using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Entities.OrderManager
{
    /// <summary>
    /// 出入库拆分订单行
    /// </summary>
    public class SubOrderRow:BaseEntity
    {
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
        /// 物料字典编号
        /// </summary>
        public int MaterialDicId { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 订单完成时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 订单物料数量
        /// </summary>
        public int PreCount { get; set; }
        /// <summary>
        /// 分拣数量
        /// </summary>
        public int? Sorting { get; set; }
        /// <summary>
        /// 实际数量
        /// </summary>
        public int? RealityCount { get; set; }

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
        /// 关联订单
        /// </summary>
        public SubOrder SubOrder { get; set; }
        /// <summary>
        /// 关联分区
        /// </summary>
        public ReservoirArea ReservoirArea { get; set; }

        /// <summary>
        /// 关联物料字典
        /// </summary>
        public MaterialDic MaterialDic { get; set; }

        /// <summary>
        /// 订单行状态
        /// </summary>
        public int? Status { get; set; }
        
        /// <summary>
        /// 关联前置订单行编号
        /// </summary>
        public int? OrderRowId { get; set; }

        public OrderRow OrderRow { get; set; }

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
        /// 集约化物资系统订单行编号
        /// </summary>
        public int SourceId { get; set; }



    }
}
