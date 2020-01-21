using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Misc;
using ApplicationCore.Entities.BasicInformation;
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
        private readonly ITransactionRepository _transactionRepository;

        public InOutTaskService(IAsyncRepository<InOutTask> inOutTaskRepository,
                                IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                                IAsyncRepository<Location> locationRepository,
                                IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                                IAsyncRepository<TrayDic> trayRespository,
                                IAsyncRepository<InOutRecord> inOutRespository,
                                IAsyncRepository<ModuleLock> moduleLockRespository,
                                ITransactionRepository transactionRepository)
        {
            this._inOutTaskRepository = inOutTaskRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._locationRepository = locationRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._trayDicRespository = trayRespository;
            this._inOutRecordRespository = inOutRespository;
            this._moduleLockRespository = moduleLockRespository;
            this._transactionRepository = transactionRepository;
        }


        public async Task EmptyAwaitInApply(string trayCode,int orgId)
        {
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));
            if (trayCode.Split('#').Length < 2)
                throw new Exception("托盘编码不合法,无法分拣!");
            WarehouseTraySpecification traySpec = new WarehouseTraySpecification(null, trayCode,
                null, null, null, null, null, null,null,null, null, null, null);
            var trayDicSpec = new TrayDicSpecification(null, trayCode.Split('#')[0], null);
            var trayDics = await this._trayDicRespository.ListAsync(trayDicSpec);
            var trayDic = trayDics[0];
            var whTrays = await this._warehouseTrayRepository.ListAsync(traySpec);
            WarehouseTray warehouseTray = new WarehouseTray
            {
                Code = trayCode,
                CreateTime = DateTime.Now,
                TrayDicId = trayDic.Id,
                MaterialCount = 0,
                TrayStep = Convert.ToInt32(TRAY_STEP.待入库),
                OrganizationId=orgId
            };
            if (whTrays.Count > 0)
            {
                var whTray = whTrays[0];
                var trayMaterials = whTray.WarehouseMaterial;
                if (trayMaterials.Count > 0)
                    throw new Exception(string.Format("托盘[{0}]不是空托盘！", trayCode));
              
                await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
            }
            else
            {
                await this._warehouseTrayRepository.AddAsync(warehouseTray);
            }
           
        }


        public async Task InApply(string trayCode,string locationCode)
        {
            Guard.Against.NullOrEmpty(trayCode, nameof(trayCode));
            WarehouseTrayDetailSpecification traySpec = new WarehouseTrayDetailSpecification(null, trayCode,
               null, null, null,null, null,null,null,null, null, null, null);
            var warehouseTrays = await this._warehouseTrayRepository.ListAsync(traySpec);
            Guard.Against.Zero(warehouseTrays.Count, nameof(warehouseTrays.Count));
            var warehouseTray = warehouseTrays[0];
            if (!warehouseTray.TrayStep.HasValue ||
                warehouseTray.TrayStep.Value != Convert.ToInt32(TRAY_STEP.待入库))
                throw new Exception("托盘状态错误,必须为待入库,当前托盘状态为["+
                    Enum.GetName(typeof(TRAY_STEP), warehouseTray.TrayStep.Value) +"]");

            LocationSpecification locationSpec = new LocationSpecification(null, locationCode,null,null,null,null,null,null,null);
            var locations=await this._locationRepository.ListAsync(locationSpec);
            Guard.Against.Zero(locations.Count, nameof(locations.Count));
            var location = locations[0];
            string srcCode = location.SysCode;
            string targetCode = string.Empty;
            BaseSpecification<Location> areaLocationSpec = null;
            if (warehouseTray.WarehouseMaterial.Count > 0)
            {
                areaLocationSpec = new LocationSpecification(null, null,null,null, null, warehouseTray.ReservoirAreaId,
                                                     null, Convert.ToInt32(LOCATION_STATUS.正常),
                                                     Convert.ToInt32(LOCATION_INSTOCK.无货));
            }
            else
            {
                areaLocationSpec = new LocationSpecification(null, null,warehouseTray.OrganizationId,null, null, null,
                                                   Convert.ToInt32(LOCATION_TYPE.仓库区货位),
                                                   Convert.ToInt32(LOCATION_STATUS.正常),
                                                   Convert.ToInt32(LOCATION_INSTOCK.无货));
            }
            var areaLocations = await this._locationRepository.ListAsync(areaLocationSpec);
            if (areaLocations.Count == 0)
                throw new Exception("所属分区没有可用的空货位");
            var areaLocation = areaLocations[0];
            targetCode = areaLocation.SysCode;
            this._transactionRepository.Transaction(async() =>
            {
                warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.入库中);
                warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.输送线);
                warehouseTray.LocationId = location.Id;
                InOutTask inOutTask = new InOutTask
                {
                    CreateTime = DateTime.Now,
                    OrderId = warehouseTray.OrderId,
                    OrderRowId = warehouseTray.OrderRowId,
                    WarehouseTrayId = warehouseTray.Id,
                    SrcId = srcCode,
                    TargetId = targetCode,
                    Status = Convert.ToInt32(TASK_STATUS.待处理),
                    Step = Convert.ToInt32(TASK_STEP.已接收),
                    Type = Convert.ToInt32(TASK_FLAG.入库),
                    IsRead=0
                };
                areaLocation.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                
                Task.WaitAll(this._warehouseTrayRepository.UpdateAsync(warehouseTray),
                             this._inOutTaskRepository.AddAsync(inOutTask),
                             this._locationRepository.UpdateAsync(areaLocation));
                
            });

        }

        public async Task AwaitOutApply(int orderId, int orderRowId, List<WarehouseTray> warehouseTrays)
        {
            Guard.Against.NullOrEmpty(warehouseTrays, nameof(warehouseTrays));
            this._transactionRepository.Transaction(() =>
            {
                warehouseTrays.ForEach(async (warehouseTray) =>
                {
                    var mls =await this._moduleLockRespository.ListAllAsync();
                    var ml = mls[0];
                    ml.UpdateTime = DateTime.Now;
                    await this._moduleLockRespository.UpdateAsync(ml);
                    WarehouseTraySpecification traySpec = new WarehouseTraySpecification(null, warehouseTray.Code,
                                               null, null, null,null,null, null, null, null, null, null, null);
                    var trays = await this._warehouseTrayRepository.ListAsync(traySpec);
                    Guard.Against.Zero(trays.Count, nameof(trays.Count));
                    var tray = trays[0];
                    if (tray.TrayStep != Convert.ToInt32(TRAY_STEP.已上架))
                        throw new Exception(string.Format("托盘[{0}],状态不是[{1}],无法出库！",tray.Code, TRAY_STEP.已上架.ToString()));
                    tray.OrderId = orderId;
                    tray.OrderRowId = orderRowId;
                    tray.TrayStep = Convert.ToInt32(TRAY_STEP.待出库);
                    tray.OutCount = warehouseTray.OutCount;
                    await this._warehouseTrayRepository.UpdateAsync(tray);
                });
            });
        }

        public async Task TaskStepReport(int id,int vid,string vName, int taskStep)
        {
            InOutTaskSpecification taskSpec = new InOutTaskSpecification(id, null,
                                              null,null,null,null,null, null, null, null, null);
            var tasks = await this._inOutTaskRepository.ListAsync(taskSpec);
            Guard.Against.NullOrEmpty(tasks, nameof(tasks));
            var task = tasks[0];
            WarehouseTrayDetailSpecification warehouseTraySpec = new WarehouseTrayDetailSpecification(task.WarehouseTrayId,
                                       null, null, null, null,null,null, null, null, null, null, null, null);
            var warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
            var warehouseTray = warehouseTrays[0];


            if (taskStep == Convert.ToInt32(TASK_STEP.任务开始))
            {
                task.Status = Convert.ToInt32(TASK_STATUS.执行中);
                task.Progress = 10;
                if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.待出库))
                {
                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库中未下架);
                    await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
                }
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
                        null, null, null, null, null, null, null);
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
                    this._transactionRepository.Transaction(async () =>
                    {

                        if (warehouseTray.MaterialCount > 0)
                        {
                            //生成出库记录
                            InOutRecord inOutRecord = new InOutRecord
                            {
                                CreateTime = DateTime.Now,
                                InOutCount = warehouseTray.OutCount,
                                IsRead = 0,
                                MaterialDicId = warehouseTray.WarehouseMaterial[0].MaterialDicId,
                                OrderId = warehouseTray.OrderId,
                                OrderRowId = warehouseTray.OrderRowId,
                                WarehouseId=warehouseTray.WarehouseId.Value,
                                ReservoirAreaId = warehouseTray.ReservoirAreaId.Value,
                                OrganizationId=warehouseTray.OrganizationId,
                                OUId=warehouseTray.OUId,
                                TrayDicId = warehouseTray.TrayDicId,
                                Type = Convert.ToInt32(INOUTRECORD_FLAG.出库)
                            };
                            warehouseTray.LocationId = null;
                            warehouseTray.Carrier = null;
                            warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库完成);
                            await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
                            await this._inOutRecordRespository.AddAsync(inOutRecord);
                        }
                        else
                        {
                            warehouseTray.LocationId = null;
                            warehouseTray.Carrier = null;
                            warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.初始化);
                            await this._warehouseTrayRepository.UpdateAsync(warehouseTray);
                        }

                    });

                }
                else if (warehouseTray.TrayStep == Convert.ToInt32(TRAY_STEP.入库中))
                {
                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.已上架);
                    warehouseTray.Carrier = Convert.ToInt32(TRAY_CARRIER.货架);

                    this._transactionRepository.Transaction(async () =>
                    {
                        if (warehouseTray.MaterialCount > 0)
                        {
                            //生成入库记录
                            InOutRecord inOutRecord = new InOutRecord
                            {
                                CreateTime = DateTime.Now,
                                InOutCount = warehouseTray.MaterialCount,
                                IsRead = 0,
                                MaterialDicId = warehouseTray.WarehouseMaterial[0].MaterialDicId,
                                OrderId = warehouseTray.OrderId,
                                OrderRowId = warehouseTray.OrderRowId,
                                ReservoirAreaId = warehouseTray.ReservoirAreaId.Value,
                                WarehouseId = warehouseTray.WarehouseId.Value,
                                OrganizationId = warehouseTray.OrganizationId,
                                OUId = warehouseTray.OUId,
                                TrayDicId = warehouseTray.TrayDicId,
                                Type = Convert.ToInt32(INOUTRECORD_FLAG.入库)
                            };
                            await this._inOutRecordRespository.AddAsync(inOutRecord);
                        }

                        LocationSpecification locationSpec = new LocationSpecification(null,
                                                             task.TargetId, null, null, null, null, null, null, null);
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

        public async Task EmptyAwaitOutApply(List<WarehouseTray> warehouseTrays)
        {
            Guard.Against.NullOrEmpty(warehouseTrays, nameof(warehouseTrays));
            this._transactionRepository.Transaction(() =>
            {
                warehouseTrays.ForEach(async (warehouseTray) =>
                {
                    var mls = await this._moduleLockRespository.ListAllAsync();
                    var ml = mls[0];
                    ml.UpdateTime = DateTime.Now;
                    await this._moduleLockRespository.UpdateAsync(ml);
                    WarehouseTraySpecification traySpec = new WarehouseTraySpecification(null, warehouseTray.Code,
                                               null, null,null,null, null, null, null, null, null, null, null);
                    var trays = await this._warehouseTrayRepository.ListAsync(traySpec);
                    Guard.Against.Zero(trays.Count, nameof(trays.Count));
                    var tray = trays[0];
                    if (tray.TrayStep != Convert.ToInt32(TRAY_STEP.已上架))
                        throw new Exception(string.Format("托盘[{0}],状态不是[{1}],无法出库！", tray.Code, TRAY_STEP.已上架.ToString()));
                    if (tray.MaterialCount > 0) throw new Exception(string.Format("托盘[{0}],不是空托盘！", tray.Code));
                    tray.TrayStep = Convert.ToInt32(TRAY_STEP.待出库);
                    tray.OutCount = warehouseTray.OutCount;
                    await this._warehouseTrayRepository.UpdateAsync(tray);
                });
            });
        }
    }
}
