using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Entities.OrderManager
{
    public class OrderRow:BaseEntity
    {
        /// <summary>
        /// 关联订单Id
        /// </summary>
        public int OrderId { get; set; }

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
        /// 取消数量
        /// </summary>
        public int? CancelCount { get; set; }

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
        public Order Order { get; set; }
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
        public int Status { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public int EBSTaskId { get; set; }
        /// <summary>
        /// 关联任务实体
        /// </summary>
        public EBSTask EBSTask { get; set; }
        
        /// <summary>
        /// 第三方系统编号
        /// </summary>
        public int? SourceId { get; set; }
        
        /// <summary>
        /// 不含税单价
        /// </summary>
        public double Price { get; set; }
        
        /// <summary>
        /// 不含税金额
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public string UseFor { get; set; }


    }
}
