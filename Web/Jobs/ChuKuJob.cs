﻿using System;
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
using ApplicationCore.Entities.TaskManager;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;
using Microsoft.EntityFrameworkCore;

namespace Web.Jobs
{
    /// <summary>
    /// 出库订单处理定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class ChuKuKJob:IJob
    {
      
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;

        public ChuKuKJob()
        {
            this._warehouseTrayRepository = EnginContext.Current.Resolve<IAsyncRepository<WarehouseTray>>();
            this._warehouseRepository = EnginContext.Current.Resolve<IAsyncRepository<Warehouse>>();
            this._reservoirAreaRepository = EnginContext.Current.Resolve<IAsyncRepository<ReservoirArea>>();
            this._locationRepository = EnginContext.Current.Resolve<IAsyncRepository<Location>>();
            this._warehouseMaterialRepository = EnginContext.Current.Resolve<IAsyncRepository<WarehouseMaterial>>();
            this._inOutTaskRepository = EnginContext.Current.Resolve<IAsyncRepository<InOutTask>>();
            this._subOrderRepository = EnginContext.Current.Resolve<IAsyncRepository<SubOrder>>();
            this._subOrderRowRepository = EnginContext.Current.Resolve<IAsyncRepository<SubOrderRow>>();
            this._logRecordRepository = EnginContext.Current.Resolve<IAsyncRepository<LogRecord>>();
        }

         public async Task Execute(IJobExecutionContext context)
         {
             using (await ModuleLock.GetAsyncLock().LockAsync())
             {
                 OrderMaterialChuKu();
                 EmptyTrayChuKu();
             }
         }

         void EmptyTrayChuKu()
         {
             try
             {
                 WarehouseTraySpecification warehouseTraySpecification = new WarehouseTraySpecification(null,
                     null,
                     new List<int> {0, 0}, null, null, null,
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
                             InOutTask inOutTask = new InOutTask();
                             inOutTask.TrayCode = tray.TrayCode;
                             inOutTask.SrcId = tray.Location.SysCode;
                             int index = random.Next(0, locations.Count - 1);
                             inOutTask.TargetId = locations[index].SysCode;
                             inOutTask.Status = Convert.ToInt32(TASK_STATUS.待处理);
                             inOutTask.Type = Convert.ToInt32(TASK_TYPE.空托盘出库);
                             inOutTask.CreateTime = DateTime.Now;
                             inOutTask.IsRead = Convert.ToInt32(TASK_READ.未读);
                             inOutTask.OUId = tray.OUId;
                             inOutTask.WarehouseId = tray.WarehouseId;
                             inOutTask.ReservoirAreaId = tray.ReservoirAreaId;
                             inOutTask.PhyWarehouseId = tray.PhyWarehouseId;

                             tray.TrayStep = Convert.ToInt32(TRAY_STEP.出库中未执行);
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
                     new List<int> {Convert.ToInt32(ORDER_TYPE.出库领料), Convert.ToInt32(ORDER_TYPE.入库退库)},
                     new List<int> {Convert.ToInt32(ORDER_STATUS.执行中)}, Convert.ToInt32(ORDER_READ.未读),
                     null, null, null, null, null, null, null, null, null,
                     null, null, null);
                 List<SubOrder> subOrders = this._subOrderRepository.List(subOrderSpecification);
                 foreach (var subOrder in subOrders)
                 {
                     using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                     {
                         try
                         {
                             SubOrderRowSpecification subOrderRowSpecification = new SubOrderRowSpecification(null,
                                 subOrder.Id, null,
                                 null, null, null, null, null, null, null, null, null,
                                 null, null, null, null, null);
                             List<SubOrderRow> subOrderRows =
                                 this._subOrderRowRepository.List(subOrderRowSpecification);

                             List<InOutTask> sendTasks = new List<InOutTask>();
                             List<WarehouseTray> warehouseTrays = new List<WarehouseTray>();
                             List<WarehouseMaterial> updMaterials = new List<WarehouseMaterial>();
                             List<Location> updLocations = new List<Location>();
                             foreach (var subOrderRow in subOrderRows)
                             {
                                 ReservoirArea area = subOrderRow.ReservoirArea;
                                 LocationSpecification locationSpecification = new LocationSpecification(null, null,
                                     null, null, null, null, null, area.Id, null,
                                     null, null, null, null, null);
                                 List<Location> locations = this._locationRepository.List(locationSpecification);
                                 if (locations.Count == 0)
                                     throw new Exception("订单[{0}],订单行[{1}],子库区[{2}],没有分配货位,请联系管理员!");
                                 Location location = locations.First();
                                 PhyWarehouse phyWarehouse = location.PhyWarehouse;
                                 locationSpecification = new LocationSpecification(null, null,
                                     null, new List<int> {Convert.ToInt32(LOCATION_TYPE.出库区货位)}, phyWarehouse.Id, null,
                                     null, null, null,
                                     null, null, null, null, null);
                                 locations = this._locationRepository.List(locationSpecification);
                                 if (locations.Count == 0)
                                     throw new Exception("没有出库区货位!");
                                 MaterialDic materialDic = subOrderRow.MaterialDic;
                                 WarehouseMaterialSpecification warehouseMaterialSpecification =
                                     new WarehouseMaterialSpecification(null, null,
                                         subOrderRow.MaterialDicId, null, null, null, null, null, null, null,
                                         new List<int>
                                         {
                                             Convert.ToInt32(TRAY_STEP.入库完成),
                                             Convert.ToInt32(TRAY_STEP.初始化)
                                         }, null, null, null, area.Id, null,
                                         null, null, null, null);
                                 List<WarehouseMaterial> warehouseMaterials =
                                     this._warehouseMaterialRepository.List(warehouseMaterialSpecification);
                                 List<WarehouseMaterial> inFinishWarehouseMaterials = warehouseMaterials
                                     .Where(m => m.WarehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.入库完成)).ToList();
                                 List<WarehouseMaterial> initWarehouseMaterials = warehouseMaterials
                                     .Where(m => m.WarehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.初始化)).ToList();
                                 var sumMaterialCount = inFinishWarehouseMaterials.Sum(m => m.MaterialCount);
                                 if (sumMaterialCount < subOrderRow.PreCount)
                                 {
                                     inFinishWarehouseMaterials.AddRange(initWarehouseMaterials);
                                     sumMaterialCount = inFinishWarehouseMaterials.Sum(m => m.MaterialCount);
                                     if (sumMaterialCount < subOrderRow.PreCount)
                                         throw new Exception("订单[{0}],订单行[{1}],物料库存数量不足,无法执行出库操作!");
                                 }
                                 
                                 var sortMaterials = inFinishWarehouseMaterials.OrderBy(m => m.CreateTime);
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
                                         end=true;
                                         warehouseTray.OutCount = -(subOrderRow.PreCount-totalCnt.Sum(t => t));
                                     }
                                     else
                                     {
                                         warehouseTray.OutCount = -sortMaterial.MaterialCount;
                                     }
                                     totalCnt.Add(sortMaterial.MaterialCount);
                                     warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库中未执行);
                                     warehouseTray.SubOrderId = subOrder.Id;
                                     warehouseTray.SubOrderRowId = subOrderRow.Id;
                                     warehouseTrays.Add(warehouseTray);
                                     sortMaterial.SubOrderId = subOrder.Id;
                                     sortMaterial.SubOrderRowId = subOrderRow.Id;
                                     updMaterials.Add(sortMaterial);
                                     InOutTask inOutTask = new InOutTask();
                                     inOutTask.SubOrderId = subOrder.Id;
                                     inOutTask.SubOrderRowId = subOrderRow.Id;
                                     inOutTask.TrayCode = sortMaterial.WarehouseTray.TrayCode;
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

                             subOrder.IsRead = Convert.ToInt32(ORDER_READ.已读);
                             this._subOrderRepository.Update(subOrder);
                             this._inOutTaskRepository.Add(sendTasks);
                             this._warehouseTrayRepository.Update(warehouseTrays);
                             this._warehouseMaterialRepository.Update(updMaterials);
                             this._locationRepository.Update(updLocations);
                             scope.Complete();
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
                     }
                 }
             }
             catch (Exception ex)
             {

             }
         }
    }
}
