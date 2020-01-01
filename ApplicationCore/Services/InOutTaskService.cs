using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Misc;
using ApplicationCore.Entities.SysManager;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities;

namespace ApplicationCore.Services
{
    public class InOutTaskService: IInOutTaskService
    {
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<TrayDic> _trayDicRespository;
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRespository;
        private readonly IAsyncRepository<ModuleLock> _moduleLockRespository;

        public InOutTaskService(IAsyncRepository<InOutTask> inOutTaskRepository,
                                IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                                IAsyncRepository<Location> locationRepository,
                                IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                                IAsyncRepository<TrayDic> trayRespository,
                                IAsyncRepository<InOutRecord> inOutRespository,
                                IAsyncRepository<ModuleLock> moduleLockRespository)
        {
            this._inOutTaskRepository = inOutTaskRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._locationRepository = locationRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._trayDicRespository = trayRespository;
            this._inOutRecordRespository = inOutRespository;
            this._moduleLockRespository = moduleLockRespository;
        }


        public async Task EmptyAwaitInApply(string trayCode, int warehouseId, int areaId)
        {
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));
            WarehouseTraySpecification traySpec = new WarehouseTraySpecification(null, trayCode,
                null, null, null, null, null, null, null, null, null);
            var whTrays = await this._warehouseTrayRepository.ListAsync(traySpec);
            if (whTrays.Count > 0)
            {
                if (whTrays[0].Carrier.HasValue)
                    throw new Exception(string.Format("空托盘已经绑定载体[{0}],无法执行待入库操作！",
                        whTrays[0].Carrier));
                else
                {
                    var whTray = whTrays[0];
                    var trayMaterials = whTray.WarehouseMaterial;
                    await this._warehouseMaterialRepository.DeleteAsync(trayMaterials);
                    await this._warehouseTrayRepository.DeleteAsync(whTray);
                }
            }
            if (trayCode.Split('#').Length < 2)
                throw new Exception("托盘编码不合法,无法分拣!");
            var trayDicSpec = new TrayDicSpecification(null, trayCode.Split('#')[0], null);
            var trayDics = await this._trayDicRespository.ListAsync(trayDicSpec);
            var trayDic = trayDics[0];
            WarehouseTray warehouseTray = new WarehouseTray
            {
                Code = trayCode,
                CreateTime = DateTime.Now,
                TrayDicId = trayDic.Id,
                MaterialCount=0,
                TrayStep = Convert.ToInt32(TRAY_STEP.待入库)
            };
            await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
        }


        public async Task InApply(string trayCode,string locationCode)
        {
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));
            WarehouseTrayDetailSpecification traySpec = new WarehouseTrayDetailSpecification(null, trayCode,
               null, null, null,null, null, null, null, null, null);
            var warehouseTrays = await this._warehouseTrayRepository.ListAsync(traySpec);
            Guard.Against.Zero(warehouseTrays.Count, nameof(warehouseTrays.Count));
            var warehouseTray = warehouseTrays[0];
            if (!warehouseTray.TrayStep.HasValue ||
                warehouseTray.TrayStep.Value != Convert.ToInt32(TRAY_STEP.待入库))
                throw new Exception("托盘状态错误,必须为待入库,当前托盘状态为["+
                    Enum.GetName(typeof(TRAY_STEP), warehouseTray.TrayStep.Value) +"]");

            LocationSpecification locationSpec = new LocationSpecification(null, locationCode,null,null,null,null,null);
            var locations=await this._locationRepository.ListAsync(locationSpec);
            Guard.Against.Zero(locations.Count, nameof(locations.Count));
            var location = locations[0];
            string srcCode = location.SysCode;
            string targetCode = string.Empty;
            LocationSpecification areaLocationSpec = new LocationSpecification(null, null, null, warehouseTray.ReservoirAreaId,
                                                     null, Convert.ToInt32(LOCATION_STATUS.正常),
                                                     Convert.ToInt32(LOCATION_INSTOCK.无货));
            var areaLocations = await this._locationRepository.ListAsync(areaLocationSpec);
            if (areaLocations.Count == 0)
                throw new Exception("所属分区没有可用的空货位");
            var areaLocation = areaLocations[0];
            targetCode = areaLocation.SysCode;
            this._inOutTaskRepository.TransactionScope(async() =>
            {
                warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.入库中);
                warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.输送线);
                warehouseTray.LocationId = location.Id;
                InOutTask inOutTask = new InOutTask
                {
                    CreateTime = DateTime.Now,
                    OrderId = warehouseTray.OrderId,
                    OrderRowId=warehouseTray.OrderRowId,
                    WarehouseTrayId = warehouseTray.Id,
                    SrcId=srcCode,
                    TargetId=targetCode,
                    Status=Convert.ToInt32(TASK_STATUS.待处理),
                    Step=Convert.ToInt32(TASK_STEP.已接收)
                };
                areaLocation.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                
                Task.WaitAll(this._warehouseTrayRepository.UpdateAsync(warehouseTray),
                             this._inOutTaskRepository.AddAsync(inOutTask),
                             this._locationRepository.UpdateAsync(areaLocation));
                
            });

        }

        public async Task AwaitOutApply(int orderId, int orderRowId, List<string> trayCodes)
        {
            Guard.Against.NullOrEmpty(trayCodes, nameof(trayCodes));
            this._warehouseTrayRepository.TransactionScope(() =>
            {
                trayCodes.ForEach(async (trayCode) =>
                {
                    var mls =await this._moduleLockRespository.ListAllAsync();
                    var ml = mls[0];
                    ml.UpdateTime = DateTime.Now;
                    await this._moduleLockRespository.UpdateAsync(ml);
                    WarehouseTraySpecification traySpec = new WarehouseTraySpecification(null, trayCode,
                                               null, null, null, null, null, null, null, null, null);
                    var trays = await this._warehouseTrayRepository.ListAsync(traySpec);
                    Guard.Against.Zero(trays.Count, nameof(trays.Count));
                    var tray = trays[0];
                    if (tray.TrayStep != Convert.ToInt32(TRAY_STEP.已上架))
                        throw new Exception(string.Format("托盘[{0}],状态不是[{1}],无法出库！",tray.Code, TRAY_STEP.已上架.ToString()));
                    tray.OrderId = orderId;
                    tray.OrderRowId = orderRowId;
                    tray.TrayStep = Convert.ToInt32(TRAY_STEP.待出库);
                    await this._warehouseTrayRepository.UpdateAsync(tray);
                });
            });
        }

        public async Task TaskStepReport(int id,int vid,string vName, int taskStep)
        {
            InOutTaskSpecification taskSpec = new InOutTaskSpecification(id, null,
                                              null, null, null, null, null);
            var tasks = await this._inOutTaskRepository.ListAsync(taskSpec);
            Guard.Against.NullOrEmpty(tasks, nameof(tasks));
            var task = tasks[0];
            WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(task.WarehouseTrayId,
                                       null, null, null, null, null, null, null, null, null, null);
            var warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
            var warehouseTray = warehouseTrays[0];


            if (taskStep == Convert.ToInt32(TASK_STEP.任务开始))
            {
                task.Status = Convert.ToInt32(TASK_STATUS.执行中);
                task.Progress = 10;
            }
            else if (taskStep == Convert.ToInt32(TASK_STEP.开始进提升机))
            {
                task.Progress = 20;
            }
            else if (taskStep == Convert.ToInt32(TASK_STEP.已出提升机))
            {
                task.Progress = 30;
            }
            else if (taskStep == Convert.ToInt32(TASK_STEP.取货完成))
            {
                task.Progress = 40;
                if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.出库中未下架))
                {
                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库中已下架);
                    LocationSpecification locationSpec = new LocationSpecification(null, task.SrcId,
                        null, null, null, null, null);
                    var locations = await this._locationRepository.ListAsync(locationSpec);
                    var location = locations[0];
                    location.Status = Convert.ToInt32(LOCATION_STATUS.正常);
                    Task.WaitAll(this._warehouseTrayRepository.UpdateAsync(warehouseTray),
                                 this._locationRepository.UpdateAsync(location));
                   
                }
            }
            else if (taskStep == Convert.ToInt32(TASK_STEP.搬运中))
            {
                task.Progress = 50;
                warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.车辆);
                await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
            }
            else if (taskStep == Convert.ToInt32(TASK_STEP.放货完成))
            {
                task.Progress = 80;
            }
            else if (taskStep == Convert.ToInt32(TASK_STEP.任务结束))
            {
                task.Status = Convert.ToInt32(TASK_STATUS.完成);
                task.Progress = 100;
                if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.出库中已下架))
                {
                    this._warehouseTrayRepository.TransactionScope(async () =>
                    {
                        if (warehouseTray.MaterialCount > 0)
                        {
                            //生成出库记录
                            InOutRecord inOutRecord = new InOutRecord
                            {
                                CreateTime = DateTime.Now,
                                InOutCount = warehouseTray.MaterialCount,
                                IsRead = 0,
                                InOutFlag = Convert.ToInt32(INOUTRECORD_FLAG.出库),
                                MaterialDicId = warehouseTray.WarehouseMaterial[0].MaterialDicId,
                                OrderId = warehouseTray.OrderId,
                                OrderRowId = warehouseTray.OrderRowId,
                                ReservoirAreaId = warehouseTray.ReservoirAreaId,
                                TrayDicId = warehouseTray.TrayDicId,
                                Type = Convert.ToInt32(INOUTRECORD_TYPE.流程)
                            };
                            await this._inOutRecordRespository.AddAsync(inOutRecord);
                        }
                        //删除库存托盘
                        await this._warehouseTrayRepository.DeleteAsync(warehouseTray);
                    });


                }
                else if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.入库中))
                {
                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.已上架);
                    warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.货架);

                    this._warehouseTrayRepository.TransactionScope(async () =>
                    {
                        if (warehouseTray.MaterialCount > 0)
                        {
                            //生成入库记录
                            InOutRecord inOutRecord = new InOutRecord
                            {
                                CreateTime = DateTime.Now,
                                InOutCount = warehouseTray.MaterialCount,
                                IsRead = 0,
                                InOutFlag = Convert.ToInt32(INOUTRECORD_FLAG.入库),
                                MaterialDicId = warehouseTray.WarehouseMaterial[0].MaterialDicId,
                                OrderId = warehouseTray.OrderId,
                                OrderRowId = warehouseTray.OrderRowId,
                                ReservoirAreaId = warehouseTray.ReservoirAreaId,
                                TrayDicId = warehouseTray.TrayDicId,
                                Type = Convert.ToInt32(INOUTRECORD_TYPE.流程)
                            };
                            await this._inOutRecordRespository.AddAsync(inOutRecord);
                        }

                        LocationSpecification locationSpec = new LocationSpecification(null,
                                                             task.TargetId, null, null, null, null, null);
                        var locations = await this._locationRepository.ListAsync(locationSpec);
                        var location = locations[0];
                        location.Status = Convert.ToInt32(LOCATION_STATUS.正常);
                        Task.WaitAll(this._warehouseTrayRepository.UpdateAsync(warehouseTray),
                                     this._locationRepository.UpdateAsync(location));
                    });
                }
            }
            task.Step = taskStep;
            task.VehicleId = vid;
            task.VehicleName = vName;
            await this._inOutTaskRepository.UpdateAsync(task);
        }
    }
}
