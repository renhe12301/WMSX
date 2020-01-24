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
        注销 = 1
    }

    public enum WAREHOUSE_STATUS
    {
        正常 = 0,
        禁用 = 1
    }

    public enum ORDER_TYPE
    {
        采购入库 = 0,
        领料出库 = 1
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
        有货=1
    }

    public enum LOCATION_TYPE
    {
        入库区货位 = 0,
        出库区货位 = 1,
        出入库区货位 = 2,
        仓库区货位 = 3
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
        入库中 = 2,
        已上架 = 3,
        待出库 = 4,
        出库中未下架 = 5,
        出库中已下架 = 6,
        出库完成待确认 = 7
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
        注销 = 1
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
        完成=2
    }

    public enum INOUTRECORD_FLAG
    {
        入库 = 0,
        出库 = 1
    }

    public enum TASK_FLAG
    {
        入库 = 0,
        出库 = 1
    }
    
   
}
