using System;
using System.Threading.Tasks;
using Quartz;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using ApplicationCore.Misc;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;
using System.Linq;
using System.Threading;
using ApplicationCore.Entities.TaskManager;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;

namespace Web.Jobs
{
    /// <summary>
    /// 出库订单处理定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class ChuKuKJob:IJob
    {
      
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;

        public ChuKuKJob(IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                         IAsyncRepository<Location> locationRepository,
                         IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                         IAsyncRepository<InOutTask> inOutTaskRepository,
                         IAsyncRepository<SubOrder> subOrderRepository,
                         IAsyncRepository<SubOrderRow> subOrderRowRepository,
                         IAsyncRepository<LogRecord> logRecordRepository)
        {
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._locationRepository = locationRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._inOutTaskRepository = inOutTaskRepository;
            this._subOrderRepository = subOrderRepository;
            this._subOrderRowRepository = subOrderRowRepository;
            this._logRecordRepository = logRecordRepository;
        }

         public async Task Execute(IJobExecutionContext context)
         {
             using (await ModuleLock.GetAsyncLock().LockAsync())
             {
                 OrderMaterialChuKu();
                 EmptyTrayChuKu();
             }
             
             Thread.Sleep(100);
         }

         void EmptyTrayChuKu()
         {
             try
             {
                 WarehouseTraySpecification warehouseTraySpecification = new WarehouseTraySpecification(null,
                     null,
                     new List<double> {0, 0}, null, null, null,
                     new List<int> {Convert.ToInt32(TRAY_STEP.待出库)}, null,
                     null, null, null, null);

                 List<WarehouseTray> warehouseTrays =
                     this._warehouseTrayRepository.List(warehouseTraySpecification);

                 Random random = new Random();

                 foreach (var tray in warehouseTrays)
                 {
                     using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                     {
                         try
                         {
                             PhyWarehouse phyWarehouse = tray.PhyWarehouse;
                             LocationSpecification locationSpecification = new LocationSpecification(null, null,
                                 null, new List<int> {Convert.ToInt32(LOCATION_TYPE.出库区货位)}, phyWarehouse.Id, null,
                                 null, null, null,
                                 null, null, null, null, null);
                             List<Location> locations = this._locationRepository.List(locationSpecification);
                             int index = random.Next(0, locations.Count - 1);
                             var srcLoc = tray.Location;
                             var tarLoc = locations[index];
                             InOutTask inOutTask = new InOutTask();
                             inOutTask.TrayCode = tray.TrayCode;
                             inOutTask.SrcId = srcLoc.SysCode;
                            
                             inOutTask.TargetId = tarLoc.SysCode;
                             inOutTask.Status = Convert.ToInt32(TASK_STATUS.待处理);
                             inOutTask.Type = Convert.ToInt32(TASK_TYPE.空托盘出库);
                             inOutTask.CreateTime = DateTime.Now;
                             inOutTask.IsRead = Convert.ToInt32(TASK_READ.未读);
                             inOutTask.OUId = tray.OUId;
                             inOutTask.WarehouseId = tray.WarehouseId;
                             inOutTask.ReservoirAreaId = tray.ReservoirAreaId;
                             inOutTask.PhyWarehouseId = tray.PhyWarehouseId;
                             inOutTask.WarehouseTrayId = tray.Id;
                             
                             srcLoc.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                             srcLoc.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                             
                             tarLoc.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                             tarLoc.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                             
                             tray.TrayStep = Convert.ToInt32(TRAY_STEP.出库中未执行);
                             this._warehouseTrayRepository.Update(tray);
                             this._inOutTaskRepository.Add(inOutTask);
                             this._locationRepository.Update(new List<Location>{srcLoc,tarLoc});
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

                 }
             }
             catch (Exception ex)
             {

             }
             
         }

         void OrderMaterialChuKu()
         {
             try
             {
                 SubOrderSpecification subOrderSpecification = new SubOrderSpecification(null, null, null,
                     new List<int> {Convert.ToInt32(ORDER_TYPE.出库领料), Convert.ToInt32(ORDER_TYPE.出库退库)},
                     null,
                     new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, Convert.ToInt32(ORDER_READ.未读),
                     null,null, null, null, null, null, null, null, null, null,
                     null, null, null);
                 List<SubOrder> subOrders = this._subOrderRepository.List(subOrderSpecification);
                 foreach (var subOrder in subOrders)
                 {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        try
                        {

                            SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(null,
                                subOrder.Id, null, null, null, null,
                                null, null, null, null, null, null, null, null, null,
                                null, null, null, null, null);
                            List<SubOrderRow> subOrderRows =
                                this._subOrderRowRepository.List(subOrderRowSpecification);

                            List<InOutTask> sendTasks = new List<InOutTask>();
                            List<WarehouseTray> warehouseTrays = new List<WarehouseTray>();
                            List<WarehouseMaterial> updMaterials = new List<WarehouseMaterial>();
                            List<Location> updLocations = new List<Location>();
                            List<LogRecord> errorLogs = new List<LogRecord>();

                            foreach (var subOrderRow in subOrderRows)
                            {
                                ReservoirArea area = subOrderRow.ReservoirArea;
                                WarehouseMaterialSpecification warehouseMaterialSpecification =
                                    new WarehouseMaterialSpecification(null, null,
                                        subOrderRow.MaterialDicId, null, null, null,
                                        null, null, null, null,
                                        new List<int>
                                        {
                                             Convert.ToInt32(TRAY_STEP.入库完成),
                                             Convert.ToInt32(TRAY_STEP.初始化)
                                        }, null, null, null, area.Id, null,
                                        null, null, null, null);
                                List<WarehouseMaterial> allWarehouseMaterials =
                                    this._warehouseMaterialRepository.List(warehouseMaterialSpecification);

                                if (allWarehouseMaterials.Count == 0)
                                {
                                    errorLogs.Add(new LogRecord
                                    {
                                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                                        LogDesc = string.Format("后置出库订单[{0}],订单行[{1}],子库区[{2}]没有对应的物料[{3}]",
                                            subOrder.Id, subOrderRow.Id, area.Id, subOrderRow.MaterialDicId),
                                        CreateTime = DateTime.Now
                                    });
                                    continue;
                                }


                                List<WarehouseMaterial> inFinishWarehouseMaterials = allWarehouseMaterials
                                    .Where(m => m.WarehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.入库完成)).ToList();

                                List<WarehouseMaterial> initWarehouseMaterials = allWarehouseMaterials
                                    .Where(m => m.WarehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.初始化)).ToList();

                                var sumMaterialCount = inFinishWarehouseMaterials.Sum(m => m.MaterialCount);
                                if (sumMaterialCount < subOrderRow.PreCount)
                                {
                                    inFinishWarehouseMaterials.AddRange(initWarehouseMaterials);
                                    sumMaterialCount = inFinishWarehouseMaterials.Sum(m => m.MaterialCount);
                                    if (sumMaterialCount < subOrderRow.PreCount)
                                    {
                                        errorLogs.Add(new LogRecord
                                        {
                                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                                            LogDesc = string.Format("订单[{0}],订单行[{1}],物料[{2}],物料库存数量不足,无法执行出库操作!",
                                            subOrder.Id, subOrderRow.Id, subOrderRow.MaterialDicId),
                                            CreateTime = DateTime.Now
                                        });
                                        continue;
                                    }
                                }

                                List<WarehouseMaterial> sortMaterials =
                                    inFinishWarehouseMaterials.OrderBy(m => m.CreateTime).ToList();

                                PhyWarehouse phyWarehouse = sortMaterials[0].PhyWarehouse;

                                LocationSpecification locationSpecification = new LocationSpecification(null, null,
                                    null, new List<int> { Convert.ToInt32(LOCATION_TYPE.出库区货位) }, phyWarehouse.Id, null,
                                    null, null, null,
                                    null, null, null, null, null);
                                List<Location> locations = this._locationRepository.List(locationSpecification);
                                if (locations.Count == 0)
                                    throw new Exception(string.Format("仓库[{0}]没有可用的出库区货位!"));
                                double totalCount = 0;
                                List<double> totalCnt = new List<double>();
                                Random random = new Random();
                                bool end = false;
                                foreach (var sortMaterial in sortMaterials)
                                {
                                    WarehouseTray warehouseTray = sortMaterial.WarehouseTray;
                                    if (end) break;
                                    totalCount += sortMaterial.MaterialCount;
                                    if (totalCount >= subOrderRow.PreCount)
                                    {
                                        end = true;
                                        warehouseTray.OutCount = -(subOrderRow.PreCount - totalCnt.Sum(t => t));
                                    }
                                    else
                                    {
                                        warehouseTray.OutCount = -sortMaterial.MaterialCount;
                                    }

                                    totalCnt.Add(sortMaterial.MaterialCount);
                                    warehouseTray.SubOrderId = subOrder.Id;
                                    warehouseTray.SubOrderRowId = subOrderRow.Id;
                                    sortMaterial.SubOrderId = subOrder.Id;
                                    sortMaterial.SubOrderRowId = subOrderRow.Id;
                                    updMaterials.Add(sortMaterial);
                                    if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.初始化))
                                    {
                                        warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库完成等待确认);
                                        warehouseTrays.Add(warehouseTray);
                                        continue;
                                    }
                                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库中未执行);
                                    warehouseTrays.Add(warehouseTray);

                                    InOutTask inOutTask = new InOutTask();
                                    inOutTask.SubOrderId = subOrder.Id;
                                    inOutTask.SubOrderRowId = subOrderRow.Id;
                                    inOutTask.TrayCode = sortMaterial.WarehouseTray.TrayCode;
                                    inOutTask.WarehouseTrayId = warehouseTray.Id;
                                    inOutTask.MaterialCode = sortMaterial.MaterialDic.MaterialCode;
                                    inOutTask.MaterialDicId = sortMaterial.MaterialDicId;
                                    Location srcLoc = sortMaterial.Location;
                                    inOutTask.SrcId = srcLoc.SysCode;
                                    srcLoc.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                    srcLoc.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                                    updLocations.Add(srcLoc);
                                    int index = random.Next(0, locations.Count - 1);
                                    Location targetLoc = locations[index];
                                    targetLoc.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                    targetLoc.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                                    updLocations.Add(targetLoc);
                                    inOutTask.TargetId = targetLoc.SysCode;
                                    inOutTask.Status = Convert.ToInt32(TASK_STATUS.待处理);
                                    inOutTask.Type = Convert.ToInt32(TASK_TYPE.物料出库);
                                    inOutTask.CreateTime = DateTime.Now;
                                    inOutTask.IsRead = Convert.ToInt32(TASK_READ.未读);
                                    inOutTask.OUId = subOrder.OUId;
                                    inOutTask.WarehouseId = subOrder.WarehouseId;
                                    inOutTask.ReservoirAreaId = subOrderRow.ReservoirAreaId;
                                    inOutTask.PhyWarehouseId = phyWarehouse.Id;
                                    sendTasks.Add(inOutTask);

                                }

                            }
                            if (errorLogs.Count > 0)
                                this._logRecordRepository.Add(errorLogs);
                            subOrder.IsRead = Convert.ToInt32(ORDER_READ.已读);
                            this._subOrderRepository.Update(subOrder);
                            this._inOutTaskRepository.Add(sendTasks);
                            this._warehouseTrayRepository.Update(warehouseTrays);
                            this._warehouseMaterialRepository.Update(updMaterials);
                            this._locationRepository.Update(updLocations);
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
                 }
             }
             catch (Exception ex)
             {

             }
         }
         

    }
}
