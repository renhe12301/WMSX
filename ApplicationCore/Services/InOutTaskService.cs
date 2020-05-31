﻿using System;
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
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        public InOutTaskService(IAsyncRepository<InOutTask> inOutTaskRepository,
                                IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                                IAsyncRepository<Location> locationRepository,
                                IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                                IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository
                                )
        {
            this._inOutTaskRepository = inOutTaskRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._locationRepository = locationRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
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
                    null, null, null, areaId,null);
                List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                foreach (var tray in warehouseTrays)
                {
                    tray.TrayStep = Convert.ToInt32(TRAY_STEP.待出库);
                }
                await this._warehouseTrayRepository.UpdateAsync(warehouseTrays);
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
                null, null, null, null, null, null, null, null,null);
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
            LocationSpecification locationSpecification = new LocationSpecification(null, fromPort, null, null,
                null, null, null, null, null, null, null, null, null, null);
            List<Location> locations = await this._locationRepository.ListAsync(locationSpecification);
            if(locations.Count==0)
                throw new Exception(string.Format("货位[{0}]不存在!",fromPort));
            WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, barCode, null, null,
                null, null, null, null, null, null, null,null);
            List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
            if (warehouseTrays.Count == 0)
                throw new Exception(string.Format("托盘码[{0}],不存在！", barCode));
            WarehouseTray warehouseTray = warehouseTrays[0];
            if (warehouseTray.TrayStep != Convert.ToInt32(TRAY_STEP.待入库))
                throw new Exception(string.Format("托盘[{0}]未进行待入库操作", barCode));
            
            WarehouseMaterialSpecification warehouseMaterialSpecification = new WarehouseMaterialSpecification(null,null,
                null,null,null,null,warehouseTray.Id,null,null,null,
                null,null,null,null,null,null,null,null,null,null);

            List<WarehouseMaterial> warehouseMaterials =
                await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpecification);

            using (await ModuleLock.GetAsyncLock().LockAsync())
            {
                Location curLocation = locations.First();
                warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.入库申请);
                warehouseTray.CargoHeight = cargoHeight;
                warehouseTray.CargoWeight = cargoWeight;
                warehouseTray.LocationId = curLocation.Id;
                warehouseTray.PhyWarehouseId = curLocation.PhyWarehouseId;
                warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.输送线);
                curLocation.InStock = warehouseTray.MaterialCount>0? Convert.ToInt32(LOCATION_INSTOCK.有货):
                                                                 Convert.ToInt32(LOCATION_INSTOCK.空托盘);
                this._warehouseTrayRepository.Update(warehouseTray);
                this._locationRepository.Update(curLocation);
                if (warehouseMaterials.Count > 0)
                {
                    warehouseMaterials.ForEach(m =>
                    {
                        m.LocationId = curLocation.Id;
                        m.PhyWarehouseId = curLocation.PhyWarehouseId;
                        m.Carrier = Convert.ToInt32(TRAY_CARRIER.输送线);
                    });
                    this._warehouseMaterialRepository.Update(warehouseMaterials);
                }
            }

           
        }

        public async Task TaskReport(int taskId,long reportTime, int taskStatus, string error)
        {
            InOutTaskSpecification taskSpec = new InOutTaskSpecification(taskId, null,null,null,null,
                                              null,null,null,null,null,null, 
                                              null,null, null, null, null);
            var tasks =  this._inOutTaskRepository.List(taskSpec);
            if (tasks.Count==0)
                throw  new Exception(string.Format("任务编号[{0}],不存在！",taskId));
            var task = tasks[0];
            
            WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null,
                                       task.TrayCode, null, null, null,null,null, 
                                       null, null, null, null,null);
            var warehouseTrays =  this._warehouseTrayRepository.List(warehouseTraySpec);
            var warehouseTray = warehouseTrays[0];
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
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

                        this._warehouseTrayRepository.Update(warehouseTray);
                        this._inOutTaskRepository.Update(task);
                    }
                    else if (taskStatus == Convert.ToInt32(TASK_STEP.取货完成))
                    {
                        if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.出库中已执行))
                        {
                            warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.已下架);
                        }

                        LocationSpecification locationSpec = new LocationSpecification(null, task.SrcId, null,
                            null, null, null, null, null, null, null, null,
                            null, null, null);
                        var locations = await this._locationRepository.ListAsync(locationSpec);
                        var location = locations[0];
                        location.Status = Convert.ToInt32(LOCATION_STATUS.正常);
                        location.IsTask = Convert.ToInt32(LOCATION_TASK.没有任务);
                        location.InStock = Convert.ToInt32(LOCATION_INSTOCK.无货);
                        warehouseTray.LocationId = null;
                        warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.车辆);
                        this._warehouseTrayRepository.Update(warehouseTray);
                        this._locationRepository.Update(location);
                        this._inOutTaskRepository.Update(task);
                    }
                    else if (taskStatus == Convert.ToInt32(TASK_STEP.放货完成))
                    {
                        task.Status = Convert.ToInt32(TASK_STATUS.完成);
                        if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.已下架))
                        {
                            warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库完成等待确认);
                        }
                        else
                        {
                            warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.入库完成);
                        }

                        LocationSpecification locationSpec = new LocationSpecification(null, task.TargetId, null,
                            null, null, null, null, null, null, null, null,
                            null, null, null);
                        var locations = await this._locationRepository.ListAsync(locationSpec);
                        var location = locations[0];
                        location.Status = Convert.ToInt32(LOCATION_STATUS.正常);
                        location.IsTask = Convert.ToInt32(LOCATION_TASK.没有任务);
                        location.InStock =  warehouseTray.MaterialCount>0? Convert.ToInt32(LOCATION_INSTOCK.有货):
                                                                           Convert.ToInt32(LOCATION_INSTOCK.空托盘);
                        warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.货位);
                        warehouseTray.LocationId = location.Id;
                        this._warehouseTrayRepository.Update(warehouseTray);
                        this._inOutTaskRepository.Update(task);
                        this._locationRepository.Update(location);
                    }
                    else
                    {
                        this._inOutTaskRepository.Update(task);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task OutConfirm(string trayCode)
        {
           Guard.Against.NullOrEmpty(trayCode,nameof(trayCode));
           WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, trayCode, null,
               null, null, null, null, null, null, null, null,null);
           List<WarehouseTray> warehouseTrays = this._warehouseTrayRepository.List(warehouseTraySpec);
           if(warehouseTrays.Count==0)
               throw new Exception(string.Format("托盘[{0}],不存在！",trayCode));
           WarehouseTray warehouseTray = warehouseTrays[0];
           if(warehouseTray.TrayStep!=Convert.ToInt32(TRAY_STEP.出库完成等待确认))
               throw new Exception(string.Format("托盘[{0}]状态不是[出库完成等待确认],无法进行出库确认操作！",
                   trayCode));
           warehouseTray.MaterialCount = warehouseTray.MaterialCount - warehouseTray.OutCount.GetValueOrDefault();
           warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.初始化);
           this._warehouseTrayRepository.Update(warehouseTray);
        }
    }
}
