using System;
using System.Collections.Generic;
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
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 订单完成时间
        /// </summary>
        public DateTime FinishTime { get; set; }
        
        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// 关联订单行
        /// </summary>
        public List<OrderRow> OrderRow { get; set; }
    }
}
