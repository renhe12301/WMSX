using System;
namespace ApplicationCore.Misc
{

    public enum ORDER_CONFIRM
    {
        未确认 = 0,
        已确认 = 1
    }

    public enum EMPLOYEE_STATUS
    {
        正常 = 0,
        禁用 = 1
    }

    public enum WAREHOUSE_STATUS
    {
        正常 = 0,
        禁用 = 1
    }

    public enum ORDER_TYPE
    {
        入库接收 = 1,
        入库退库 = 2,
        出库领料 = 3,
        出库退料 = 4
    }
    
    public enum ORDER_STATUS
    {
        待处理=0,
        执行中=1,
        完成=2,
        关闭=3
    }

    public enum AREA_STATUS
    {
        正常 = 0,
        禁用 = 1
    }

    public enum LOCATION_STATUS
    {
        正常 = 0,
        锁定=1,
        禁用 = 2
    }
    public enum LOCATION_INSTOCK
    {
        无货=0,
        有货=1,
        空托盘=2
    }

    public enum LOCATION_TYPE
    {
        仓库区货位 = 0,
        入库区货位 = 1,
        出库区货位 = 2
    }
    
    public enum LOCATION_TASK
    {
        没有任务= 0,
        有任务=1
    }

    public enum TRAY_CARRIER
    {
        输送线 = 0,
        车辆 = 1,
        货位=2,
        货架 = 3
    }

    public enum TRAY_STEP
    {
        初始化=0,
        待入库 = 1,
        入库申请 = 2,
        入库中未执行 = 3,
        入库中已执行 = 4,
        入库完成 = 5,
        待出库 = 6,
        出库中未执行 = 7,
        出库中已执行 = 8,
        已下架 = 9,
        出库完成等待确认 = 10
    }

    public enum AREA_TYPE
    {
        仓库库区 = 0,
        入库库区 = 1,
        出库库区 = 2
    }

    public enum SYSUSER_STATUS
    {
        正常 = 0,
        注销 = 1
    }

    public enum SYSROLE_STATUS
    {
        正常 = 0,
        禁用 = 1
    }

    public enum TASK_STEP
    {
        已接收=0,
        任务开始=1,
        开始进提升机=2,
        已出提升机=3,
        取货完成=4,
        搬运中=5,
        放货完成=6,
        任务中断=7,
        任务结束=8
    }

    public enum TASK_STATUS
    {
        待处理=0,
        执行中=1,
        完成=2,
        终止=3
    }
    
    public enum TASK_TYPE
    {
        物料入库=0,
        物料出库=1,
        空托盘入库=2,
        空托盘出库=3
    }

    public enum INOUTRECORD_FLAG
    {
        入库 = 0,
        出库 = 1
    }

    public enum LOG_TYPE
    {
        登录日志 = 0,
        操作日志 = 1,
        WebService调用日志 = 2,
        定时任务日志 = 3,
        异常日志 = 4
    }
    
    public enum ORDER_BATCH_TYPE
    {
        立体库 = 0,
        立体库剩余物料返库 = 1,
        立体库加剩余物料返库加入库单 = 2,
        入库单 = 3
    }
    public enum ORDER_BATCH_SYNC
    {
        未同步 = 0,
        已同步 = 1
    }
    public enum ORDER_BATCH_READ
    {
        未读 = 0,
        已读 = 1
    }


}
