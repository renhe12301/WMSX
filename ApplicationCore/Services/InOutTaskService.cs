using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Misc;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Services
{
    public class InOutTaskService: IInOutTaskService
    {
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<OrderRowBatch> _orderRowBatchRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRepository;
        public InOutTaskService(IAsyncRepository<InOutTask> inOutTaskRepository,
                                IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                                IAsyncRepository<Location> locationRepository,
                                IAsyncRepository<OrderRowBatch> orderRowBatchRepository,
                                IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                                IAsyncRepository<InOutRecord> inOutRecordRepository)
        {
            this._inOutTaskRepository = inOutTaskRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._locationRepository = locationRepository;
            this._orderRowBatchRepository = orderRowBatchRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._inOutRecordRepository = inOutRecordRepository;
        }
        
        public async Task EmptyOut(int areaId,int outCount)
        {
            Guard.Against.Zero(areaId, nameof(areaId));
            Guard.Against.Zero(outCount, nameof(outCount));
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(areaId, null,
                    null, null, null, null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                if (areas.Count == 0) throw new Exception(string.Format("子库存编号[{0}],不存在！", areaId));
                WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, null,
                    new List<int> {0, 0}, null, null, null,
                    new List<int> {Convert.ToInt32(TRAY_STEP.入库完成)},
                    null, null, null, areaId);
                List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                OrderRowBatchSpecification orderRowBatchSpec = new OrderRowBatchSpecification(0);
                List<OrderRowBatch> orderRowBatches = await this._orderRowBatchRepository.ListAsync(orderRowBatchSpec);
                orderRowBatches.RemoveAll(orb => orb.OrderId.HasValue);
                List<OrderRowBatch> areaOrderRowBatches = orderRowBatches.FindAll(orb => orb.ReservoirAreaId == areaId);
                int awaitOutCount = areaOrderRowBatches.Sum(orb => orb.BatchCount);
                if((warehouseTrays.Count-awaitOutCount)<outCount)throw new Exception(
                    string.Format("子库区[{0}]空托盘库存数量{1},小于出库需求数量！",areas,warehouseTrays.Count-awaitOutCount));
                
                OrderRowBatch orderRowBatch = new OrderRowBatch
                {
                    ReservoirAreaId = areaId,
                    BatchCount = outCount,
                    CreateTime = DateTime.Now,
                    IsRead = 0,
                    Type = 0
                };
                await this._orderRowBatchRepository.AddAsync(orderRowBatch);
            }

        }

        public async Task EmptyEntry(string trayCode, int areaId)
        {
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));
            Guard.Against.Zero(areaId, nameof(areaId));

            ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(areaId, null, null,
                null, null, null);
            List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
            if (areas.Count == 0) throw new Exception(string.Format("子库存[{0}],不存在！", areaId));
            WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, trayCode, null,
                null, null, null, null, null, null, null, null);
            List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
            if (warehouseTrays.Count > 0)
            {
                WarehouseTray warehouseTray = warehouseTrays[0];
                if (warehouseTray.MaterialCount > 0)
                {
                    throw new Exception(string.Format("托盘[{0}]有物料,无法空盘入库！", trayCode));
                }

                if (warehouseTray.TrayStep != Convert.ToInt32(TRAY_STEP.初始化))
                {
                    throw new Exception(string.Format("托盘[{0}]状态未初始化,当前状态为[{1}]！",
                        trayCode, Enum.GetName(typeof(TRAY_STEP), warehouseTray.TrayStep)));
                }

                warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.待入库);
                await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
            }
            else
            {
                WarehouseTray warehouseTray = new WarehouseTray
                {
                    TrayCode = trayCode,
                    CreateTime = DateTime.Now,
                    MaterialCount = 0,
                    TrayStep = Convert.ToInt32(TRAY_STEP.待入库),
                    ReservoirAreaId = areaId,
                    OUId = areas[0].OUId,
                    WarehouseId = areas[0].WarehouseId
                };
                await this._warehouseTrayRepository.AddAsync(warehouseTray);
            }
            
        }

        public async Task EntryApply(string fromPort, string barCode, int cargoHeight, string cargoWeight)
        {
            Guard.Against.NullOrEmpty(fromPort, nameof(fromPort));
            Guard.Against.NullOrEmpty(barCode, nameof(barCode));
            WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, barCode, null, null,
                null, null, null, null, null, null, null);
            List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
            if (warehouseTrays.Count == 0)
                throw new Exception(string.Format("托盘码[{0}],不存在！", barCode));
            WarehouseTray warehouseTray = warehouseTrays[0];
            if (warehouseTray.TrayStep != Convert.ToInt32(TRAY_STEP.待入库))
                throw new Exception(string.Format("托盘[{0}]未进行待入库操作", barCode));
            warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.入库申请);
            warehouseTray.CargoHeight = cargoHeight;
            warehouseTray.CargoWeight = cargoWeight;
            await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
        }

        public async Task TaskReport(int taskId,long reportTime, int taskStatus,string error)
        {
            InOutTaskSpecification taskSpec = new InOutTaskSpecification(taskId, null,null,
                                              null,null,null,null,null, 
                                              null, null, null, null);
            var tasks = await this._inOutTaskRepository.ListAsync(taskSpec);
            if (tasks.Count==0)
                throw  new Exception(string.Format("任务编号[{0}],不存在！",taskId));
            var task = tasks[0];
            
            WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null,
                                       task.TrayCode, null, null, null,null,null, 
                                       null, null, null, null);
            var warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
            var warehouseTray = warehouseTrays[0];
            InOutRecordSpecification inOutRecordSpec = new InOutRecordSpecification(task.TrayCode,null,null,null,
                null,null,null,null,
                     new List<int>{Convert.ToInt32(ORDER_STATUS.待处理),Convert.ToInt32(ORDER_STATUS.执行中)},
                    null,null,null,null );
            List<InOutRecord> inOutRecords = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
            task.Step = taskStatus;
            if (taskStatus == Convert.ToInt32(TASK_STEP.任务开始))
            {
                task.Status = Convert.ToInt32(TASK_STATUS.执行中);
                if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.出库中未执行))
                {
                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库中已执行);
                }
                if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.入库中未执行))
                {
                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.入库中已执行);
                }
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    
                    if (inOutRecords.Count > 0)
                    {
                        InOutRecord inOutRecord = inOutRecords[0];
                        inOutRecord.Status = Convert.ToInt32(ORDER_STATUS.执行中);
                        this._inOutRecordRepository.Update(inOutRecord);
                    }
                     this._warehouseTrayRepository.Update(warehouseTray);
                     this._inOutTaskRepository.Update(task);
                     scope.Complete();
                }
            }
            else if (taskStatus == Convert.ToInt32(TASK_STEP.取货完成))
            {
                if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.出库中已执行))
                {
                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.已下架);
                }
               
                LocationSpecification locationSpec = new LocationSpecification(null, task.SrcId,null,
                    null, null,null, null, null,  null, null,null,
                    null,null,null);
                var locations = await this._locationRepository.ListAsync(locationSpec);
                var location = locations[0];
                location.Status = Convert.ToInt32(LOCATION_STATUS.正常);
                warehouseTray.LocationId = null;
                warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.车辆);
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                     this._warehouseTrayRepository.Update(warehouseTray);
                     this._locationRepository.Update(location);
                     this._inOutTaskRepository.Update(task);
                     scope.Complete();
                }
            }
            else if (taskStatus == Convert.ToInt32(TASK_STEP.放货完成))
            {
                task.Status = Convert.ToInt32(TASK_STATUS.完成);
                if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.已下架))
                {
                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库完成等待确认);
                }
                LocationSpecification locationSpec = new LocationSpecification(null, task.TargetId,null,
                    null, null,null, null, null,  null, null,null,
                    null,null,null);
                var locations = await this._locationRepository.ListAsync(locationSpec);
                var location = locations[0];
                location.Status = Convert.ToInt32(LOCATION_STATUS.正常);
                warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.货位);
                warehouseTray.LocationId = location.Id;
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (inOutRecords.Count > 0)
                    {
                        InOutRecord inOutRecord = inOutRecords[0];
                        inOutRecord.Status = Convert.ToInt32(ORDER_STATUS.完成);
                        this._inOutRecordRepository.Update(inOutRecord);
                    }
                    this._warehouseTrayRepository.Update(warehouseTray);
                    this._inOutTaskRepository.Update(task);
                    this._locationRepository.Update(location);
                    scope.Complete();
                }
                
            }
            else
            {
                this._inOutTaskRepository.Update(task);
            }
        }

        public async Task OutConfirm(string trayCode)
        {
           Guard.Against.NullOrEmpty(trayCode,nameof(trayCode));
           WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, trayCode, null,
               null, null, null, null, null, null, null, null);
           List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
           if(warehouseTrays.Count==0)
               throw new Exception(string.Format("托盘[{0}],不存在！",trayCode));
           WarehouseTray warehouseTray = warehouseTrays[0];
           if(warehouseTray.TrayStep!=Convert.ToInt32(TRAY_STEP.出库完成等待确认))
               throw new Exception(string.Format("托盘[{0}]状态不是[出库完成等待确认],无法进行出库确认操作！",
                   trayCode));
           warehouseTray.MaterialCount = warehouseTray.MaterialCount - warehouseTray.OutCount;
           warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.初始化);
           await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
        }
    }
}
