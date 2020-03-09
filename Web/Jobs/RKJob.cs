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

namespace Web.Jobs
{
    /// <summary>
    /// 入库订单处理定时任务
    /// </summary>
    public class RKJob:IJob
    {

        private IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private IAsyncRepository<Location> _locationRepository;
        private IAsyncRepository<InOutTask> _inOutTaskRepository;

        public RKJob(IAsyncRepository<WarehouseTray> warehouseTrayRepository, IAsyncRepository<Location> locationRepository, IAsyncRepository<InOutTask> inOutTaskRepository) 
        {
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._locationRepository = locationRepository;
            this._inOutTaskRepository = inOutTaskRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    //扫码设备中，读到当前入库申请的托盘信息。
                    WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, null,
                                                                  null, null, null, null, new List<int> { Convert.ToInt32(TRAY_STEP.入库申请)},
                                                                  null, null, null, null,null);
                    //得到所有数据库中 托盘状态为入库申请的托盘信息
                    List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                    //定义需要更更改入库中状态的任务数据集合
                    List<WarehouseTray> updTrays = new List<WarehouseTray>();
                    //定义需要加入inoutTask中的数据集合
                    List<InOutTask> addTasks = new List<InOutTask>();
                    //定义需要加入Location中的数据集合
                    List<Location> uplocations = new List<Location>();

                    warehouseTrays.ForEach(async tray =>
                    {
                        try
                        {
                            //任务的起始地址为数据库中的PDA生成任务的起始目的点
                            string srcId = tray.Location.SysCode;
                            //任务的起始库存区域为数据库中的PDA生成任务的起始库存区域
                            int areaId = tray.ReservoirAreaId.Value;

                            //查找该区域中的空托盘的集合  货位状态正常，货位无货，货物没有任务
                            LocationSpecification locationSpec = new LocationSpecification(null, null, null, null, null, null,
                                                                                            null,areaId,
                                                                                            new List<int> { Convert.ToInt32(LOCATION_STATUS.正常) },
                                                                                            new List<int> { Convert.ToInt32(LOCATION_INSTOCK.无货) },
                                                                                            new List<int> { Convert.ToInt32(LOCATION_TASK.没有任务) },
                                                                                            null, null, null);
                            //List<Location> locations = new List<Location>();
                            //按照货物的时间排序，生成目标任务坐标点
                            List<Location> locations = await this._locationRepository.ListAsync(locationSpec);

                            //排序
                            locations = locations.OrderBy(t => t.CreateTime).ToList();
                            //引用
                            Location location = locations[0];



                            InOutTask inOutTask = new InOutTask();
                            inOutTask.SrcId = srcId;
                            inOutTask.TargetId = location.SysCode;
                            inOutTask.CreateTime = DateTime.Now; 
                            inOutTask.TrayCode = tray.TrayCode;
                            inOutTask.Type = Convert.ToInt32(TASK_TYPE.物料出库);
                            inOutTask.Status = Convert.ToInt32(TASK_STATUS.待处理);

                            if (tray.OrderId.HasValue)
                            {
                                inOutTask.OrderId = tray.OrderId;
                                inOutTask.OrderRowId = tray.OrderRowId;

                            }


                            tray.TrayStep = Convert.ToInt32(TRAY_STEP.入库中未执行);


                            location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                            //location.InStock = Convert.ToInt32(LOCATION_INSTOCK.有货);
                            location.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);



                            updTrays.Add(tray);
                            addTasks.Add(inOutTask);
                            uplocations.Add(location);

                        }
                        catch (Exception ex) { }


                        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            try
                            {
                                this._locationRepository.Update(uplocations);
                                this._warehouseTrayRepository.Update(updTrays);
                                this._inOutTaskRepository.Add(addTasks);
                            }
                            catch (Exception ex)
                            {

                                throw ex;
                            }
                        }


                        //add数据清除
                        try
                        {
                            updTrays.Clear();
                            addTasks.Clear();
                            uplocations.Clear();
                          
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
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