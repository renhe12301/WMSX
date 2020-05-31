using System;
using System.Threading.Tasks;
using ApplicationCore.Misc;
using Quartz;
using ApplicationCore.Misc;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using ApplicationCore.Entities.StockManager;
using System.Collections.Generic;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Entities.BasicInformation;
using System.Net;
using System.Transactions;
using System.Linq;
using ApplicationCore.Entities.FlowRecord;

namespace Web.Jobs
{
    /// <summary>
    /// 入库订单处理定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class RuKuJob : IJob
    {

        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;

        public RuKuJob()
        {
            this._warehouseTrayRepository = EnginContext.Current.Resolve<IAsyncRepository<WarehouseTray>>();
            this._locationRepository = EnginContext.Current.Resolve<IAsyncRepository<Location>>();
            this._inOutTaskRepository = EnginContext.Current.Resolve<IAsyncRepository<InOutTask>>();
            this._logRecordRepository = EnginContext.Current.Resolve<IAsyncRepository<LogRecord>>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    //扫码设备中，读到当前入库申请的托盘信息。
                    WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, null,
                        null, null, null, null,
                        new List<int> {Convert.ToInt32(TRAY_STEP.入库申请)},
                        null, null, null, null, null);
                    //得到所有数据库中 托盘状态为入库申请的托盘信息
                    List<WarehouseTray> warehouseTrays = this._warehouseTrayRepository.List(warehouseTraySpec);

                    warehouseTrays.ForEach(async tray =>
                    {
                        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            try
                            {
                                //任务的起始地址为数据库中的PDA生成任务的起始目的点
                                string srcId = tray.Location.SysCode;
                                //任务的起始库存区域为数据库中的PDA生成任务的起始库存区域
                                int areaId = tray.ReservoirAreaId.Value;

                                LocationSpecification locationSpec = new LocationSpecification(null, null, null, null,
                                    null,
                                    null,
                                    null, areaId,
                                    new List<int> {Convert.ToInt32(LOCATION_STATUS.正常)},
                                    new List<int> {Convert.ToInt32(LOCATION_INSTOCK.无货)},
                                    new List<int> {Convert.ToInt32(LOCATION_TASK.没有任务)},
                                    null, null, null);

                                List<Location> locations = this._locationRepository.List(locationSpec);
                                if (locations.Count == 0)
                                    throw new Exception(string.Format("子库区[{0}]下没有可用的空货位!", areaId));
                                //引用
                                Location location = locations[0];
                                InOutTask inOutTask = new InOutTask();
                                inOutTask.SrcId = srcId;
                                inOutTask.TargetId = location.SysCode;
                                inOutTask.CreateTime = DateTime.Now;
                                inOutTask.TrayCode = tray.TrayCode;
                                inOutTask.Type = tray.MaterialCount==0?Convert.ToInt32(TASK_TYPE.空托盘入库):Convert.ToInt32(TASK_TYPE.物料入库);
                                inOutTask.Status = Convert.ToInt32(TASK_STATUS.待处理);
                                inOutTask.PhyWarehouseId = location.PhyWarehouseId;
                                inOutTask.WarehouseTrayId = tray.Id;
                                inOutTask.OUId = tray.OUId;
                                inOutTask.WarehouseId = tray.WarehouseId;
                                inOutTask.ReservoirAreaId = tray.ReservoirAreaId;
                                inOutTask.IsRead = Convert.ToInt32(TASK_READ.未读);
                                if (tray.SubOrderId.HasValue)
                                {
                                    inOutTask.SubOrderId = tray.SubOrderId;
                                    inOutTask.SubOrderRowId = tray.SubOrderRowId;
                                }

                                tray.TrayStep = Convert.ToInt32(TRAY_STEP.入库中未执行);
                                // 入库起点货位锁定
                                tray.Location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                tray.Location.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                                // 入库目标货位锁定
                                location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                location.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);

                                this._locationRepository.Update(location);
                                this._locationRepository.Update(tray.Location);
                                this._warehouseTrayRepository.Update(tray);
                                this._inOutTaskRepository.Add(inOutTask);
                            }
                            catch (Exception ex)
                            {
                                this._logRecordRepository.Add(new LogRecord
                                {
                                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                                    LogDesc = ex.Message,
                                    CreateTime = DateTime.Now
                                });

                            }
                            scope.Complete();
                        }
                    });
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}