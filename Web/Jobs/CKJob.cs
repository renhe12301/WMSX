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
using ApplicationCore.Entities.TaskManager;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;

namespace Web.Jobs
{
    /// <summary>
    /// 出库订单处理定时任务
    /// </summary>
    public class CKJob:IJob
    {
      
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<OrderRowBatch> _orderRowBathRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRepository;

        public CKJob(IAsyncRepository<WarehouseTray> warehouseTrayRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<ReservoirArea> reservoirAreaRepository,
            IAsyncRepository<Location> locationRepository,
            IAsyncRepository<OrderRowBatch> orderRowBathRepository,
             IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
             IAsyncRepository<InOutTask> inOutTaskRepository,
             IAsyncRepository<InOutRecord> inOutRecordRepository
            )
        {
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._warehouseRepository = warehouseRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._locationRepository = locationRepository;
            this._orderRowBathRepository = orderRowBathRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._inOutTaskRepository = inOutTaskRepository;
            this._inOutRecordRepository = inOutRecordRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {
                    #region 参数定义
                    //定义需要更更改入库中状态的任务数据集合
                    List<WarehouseTray> updTrays = new List<WarehouseTray>();
                    //定义需要加入inoutTask中的数据集合
                    List<InOutTask> addTasks = new List<InOutTask>();
                    //定义需要加入Location中的数据集合
                    List<Location> uplocations = new List<Location>();
                    //定义需要更改订单数据集合
                    List<OrderRowBatch> upOrderRows = new List<OrderRowBatch>();
                    //定义需要更改仓库实体类的集合

                    //定义需要更改库区实体类的集合
                    List<ReservoirArea> upReservoirAreas = new List<ReservoirArea>();
                    //定义需要更改物料

                    //定义需要更改仓库材料的集合
                    List<WarehouseMaterial> upWarehouseMaterials = new List<WarehouseMaterial>();
                    //定义需要增加的inoutRecord信息
                    List<InOutRecord> addInOutRecords = new List<InOutRecord>();
                    #endregion


                 




                    #region 查找订单
                    //根据订单得到订单信息
                    OrderRowBatchSpecification orderRowBatchSpec = new OrderRowBatchSpecification(null,null,null,null,
                        Convert.ToInt32(ORDER_BATCH_READ.未读),null,null,null);
                    //根据订单信息查找数据，获得本次订单的区域  数量和订单信息；
                    List<OrderRowBatch> orderRowBatches = await this._orderRowBathRepository.ListAsync(orderRowBatchSpec);
                    #endregion
                    //开始
                    orderRowBatches.ForEach(async (orb) =>
                    {

                        try
                        {
                            List<WarehouseMaterial> updMaterials = new List<WarehouseMaterial>();
                            int areaId = orb.ReservoirAreaId;//区域
                            int materialId = orb.OrderRow.MaterialDicId;//物料编号
                            int batchCount = orb.BatchCount; //得到本次订单的数量
                            int type = orb.Type;//得到本次出库的客户需求
                            

                            if (orb.OrderRowId.HasValue)
                            {
                                //type = 1
                                ///如果type == 1 先找到初始化的托盘，绑定相关的物料，记录剩下的状态，并且把托盘状态改成出库完成，
                                ///将数据清除，不需要增加到inoutTask中。
                                if (type == 1)
                                {
                                    //1.查找该区域下所有满足物料编号的仓库物料信息
                                    WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null, materialId.ToString(),
                                                                                                                              null, null, null, null, null, null, null, null,
                                                                                                                              new List<int>() { Convert.ToInt32(TRAY_STEP.初始化)}, null, null, null,
                                                                                                                              areaId,null,null,null);
                                    //查找符合条件的分区下的库存物料
                                    List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);

                                    if (warehouseMaterials.Count < 1)
                                        throw new Exception(string.Format("剩余物料中不存在当前物料，无法完成剩余物料出库。当前期望物料为：{0}",materialId));


                                    //2.查找托盘数据当前区域和托盘状态为初始化中的托盘；
                                    //托盘增加更新
                                    WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, null,
                                                                null, null, null, null, new List<int> { Convert.ToInt32(TRAY_STEP.初始化) },
                                                                null, null, null, areaId,null);
                                    //得到所有数据库中 托盘状态为入库申请的托盘信息
                                    List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);

                                  



                                    //2.循环遍历托盘结合，更改状态和数据
                                    foreach (var warehouseMaterial in warehouseMaterials)
                                    {
                                        //遍历托盘属性，获得托盘码和剩余数量。

                                        if (!warehouseTrays.Exists(t => t.TrayCode == warehouseMaterial.WarehouseTray.TrayCode))
                                            throw new Exception(string.Format("剩余物料中不存在托盘存放当前物料，无法完成剩余物料出库。请检查托盘码对应关系,当前遍历托盘码：{0}", warehouseMaterial.WarehouseTray.TrayCode));

                                        WarehouseTray warehouseTray = warehouseTrays.Find(t => t.TrayCode == warehouseMaterial.WarehouseTray.TrayCode);                 
                                        //1.更新warehouseTray
                                        //判断：如果batchCount小于当前托盘的count，则出的数量为batchCount，否则，全出；
                                        int outCount = batchCount < warehouseTray.MaterialCount ? batchCount : warehouseTray.MaterialCount;
                                        warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库完成等待确认);
                                        warehouseTray.OutCount = outCount;
                                        warehouseTray.Order = orb.Order;
                                        warehouseTray.OrderId = orb.OrderId;
                                        warehouseTray.Location.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                                        //warehouseTray.Location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                        updTrays.Add(warehouseTray);


                                        //2.更新inoutRecord
                                        //需要增加的心的inoutrecord
                                        InOutRecord inOutRecord = new InOutRecord();
                                        inOutRecord.CreateTime = DateTime.Now;
                                        inOutRecord.IsRead = 0;
                                        inOutRecord.MaterialDic = warehouseMaterial.MaterialDic;
                                        inOutRecord.MaterialDicId = warehouseMaterial.MaterialDicId;
                                        inOutRecord.Order = warehouseMaterial.Order;
                                        inOutRecord.OrderId = warehouseMaterial.OrderId.GetValueOrDefault();
                                        inOutRecord.OrderRow = warehouseMaterial.OrderRow;
                                        inOutRecord.OrderRowId = warehouseMaterial.OrderRowId.GetValueOrDefault();
                                        inOutRecord.OU = warehouseMaterial.OU;
                                        inOutRecord.OUId = warehouseMaterial.OUId;
                                        inOutRecord.ReservoirArea = warehouseMaterial.ReservoirArea;
                                        inOutRecord.ReservoirAreaId = warehouseMaterial.ReservoirAreaId.Value;
                                        inOutRecord.Status = 0;
                                        inOutRecord.InOutCount = outCount;
                                        inOutRecord.TrayCode = warehouseMaterial.WarehouseTray.TrayCode;
                                        inOutRecord.Type = Convert.ToInt32(INOUTRECORD_FLAG.出库);
                                        inOutRecord.Warehouse = warehouseMaterial.Warehouse;
                                        inOutRecord.WarehouseId = warehouseMaterial.WarehouseId.Value;
                                        //inOutRecord.增加
                                        addInOutRecords.Add(inOutRecord);

                                        batchCount -= outCount;

                                        if (batchCount <= 0) break;
                                    }

                                }


                                //以此往下，是剩余物料清理完之后，重新出库的流程。如果 剩余的物料小于 0 退出，不需要继续出库
                                if (batchCount > 0)
                                {
                                    //查找当前location的条件
                                    LocationSpecification locationSpec = new LocationSpecification(null, null, 
                                                                                             null, null, null, null,null,
                                                                                                            areaId,
                                                                                                            new List<int> { Convert.ToInt32(LOCATION_STATUS.正常) },
                                                                                                            new List<int> { Convert.ToInt32(LOCATION_INSTOCK.有货) },
                                                                                                            new List<int> { Convert.ToInt32(LOCATION_TASK.没有任务) },
                                                                                                            null, null, null);
                                    //List<Location> locations = new List<Location>();
                                    //查找当前所有的站点信息
                                    List<Location> locations = await this._locationRepository.ListAsync(locationSpec);

                                    //查找该区域下所有满足物料编号的仓库物料信息
                                    WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null, materialId.ToString(),
                                                                                                                              null, null, null, null, null, null, null, null,
                                                                                                                              new List<int>() { Convert.ToInt32(TRAY_STEP.入库完成)}, null, null, null,
                                                                                                                              areaId,null,null,null);
                                    //查找符合条件的分区下的库存物料
                                    List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);

                                    //去除符合条件中已存在任务的物料库存
                                    warehouseMaterials.RemoveAll(o =>
                                    {
                                        //货位状态不等于正常 任务状态不等于完成  托盘无货的全部剔除
                                        if (o.Location.Status != Convert.ToInt32(LOCATION_STATUS.正常) || o.Location.IsTask != Convert.ToInt32(TASK_STATUS.完成) || o.Location.InStock == Convert.ToInt32(LOCATION_INSTOCK.无货))
                                            return true;
                                        return false;


                                    });

                                    //按照数量进行排序：降序排序将数量最多的记下来 
                                    warehouseMaterials.OrderByDescending(a => a.MaterialCount);//count

                                    //TODO ： 判断数量 = 0 报错 
                                    if (warehouseMaterials.Count < 1)
                                    {
                                        throw new Exception(string.Format("子库区{0}下物料编号{1}库存不足", areaId, materialId));
                                    }

                                    ///确定出库的物理地址点
                                    //确定一个出库区的位置
                                    WarehouseMaterial warehousematerial = warehouseMaterials[0];
                                    //获取物理仓库地址
                                    int wlckbh = warehousematerial.Location.PhyWarehouseId.Value;//物理仓库值。
                                                                                                 //取出该物理地址中的第一个出库location位置，暂时没有增加锁定标志。
                                    Location ckLocation = locations.Find(l => l.PhyWarehouseId == wlckbh && l.Type == Convert.ToInt32(LOCATION_TYPE.出库区货位));
                                    //获得出库期望目标点
                                    string tarId = ckLocation.SysCode;

                                    foreach (var warehouseMaterial in warehouseMaterials)
                                    {
                                        warehouseMaterial.Order = orb.Order;
                                        warehouseMaterial.OrderId = (int)orb.OrderId;

                                        //托盘增加更新
                                        WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, warehouseMaterial.WarehouseTray.TrayCode,
                                                                    null, null, null, null, new List<int> { Convert.ToInt32(TRAY_STEP.入库完成) },
                                                                    null, null, null, null,null);
                                        //得到所有数据库中 托盘状态为入库申请的托盘信息
                                        List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);

                                        WarehouseTray warehouseTray = warehouseTrays[0];
                                        //判断：如果batchCount小于当前托盘的count，则出的数量为batchCount，否则，全出；
                                        int outCount = batchCount < warehouseMaterial.MaterialCount ? batchCount : warehouseMaterial.MaterialCount;
                                        warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.待出库);
                                        warehouseTray.OutCount = outCount;
                                        warehouseTray.Order = warehouseMaterial.Order;
                                        warehouseTray.OrderId = warehouseMaterial.OrderId;
                                        warehouseTray.Location.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                                        warehouseTray.Location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                        updTrays.Add(warehouseTray);

                                        //需要增加的新的inoutTask
                                        InOutTask inOutTask = new InOutTask();
                                        inOutTask.TargetId = tarId;//目标点
                                        inOutTask.SrcId = warehouseMaterial.Location.SysCode;//起始点
                                        inOutTask.CreateTime = DateTime.Now;//创建时间
                                        inOutTask.Type = Convert.ToInt32(TASK_TYPE.物料出库);
                                        inOutTask.Order = warehouseMaterial.Order;
                                        inOutTask.OrderId = warehouseMaterial.OrderId;
                                        inOutTask.TrayCode = warehouseMaterial.WarehouseTray.TrayCode;
                                        inOutTask.Status = Convert.ToInt32(TASK_STATUS.待处理);
                                        //inOutTask.Step = Convert.ToInt32(TASK_STEP.);
                                        addTasks.Add(inOutTask);
                                        //
                                        //需要增加的心的inoutrecord
                                        InOutRecord inOutRecord = new InOutRecord();
                                        inOutRecord.CreateTime = DateTime.Now;
                                        inOutRecord.IsRead = 0;
                                        inOutRecord.MaterialDic = warehouseMaterial.MaterialDic;
                                        inOutRecord.MaterialDicId = warehouseMaterial.MaterialDicId;
                                        inOutRecord.Order = warehouseMaterial.Order;
                                        inOutRecord.OrderId = warehouseMaterial.OrderId.GetValueOrDefault();
                                        inOutRecord.OrderRow = warehouseMaterial.OrderRow;
                                        inOutRecord.OrderRowId = warehouseMaterial.OrderRowId.GetValueOrDefault();
                                        inOutRecord.OU = warehouseMaterial.OU;
                                        inOutRecord.OUId = warehouseMaterial.OUId;
                                        inOutRecord.ReservoirArea = warehouseMaterial.ReservoirArea;
                                        inOutRecord.ReservoirAreaId = warehouseMaterial.ReservoirAreaId.Value;
                                        inOutRecord.Status = 0;
                                        inOutRecord.InOutCount = outCount;
                                        inOutRecord.TrayCode = warehouseMaterial.WarehouseTray.TrayCode;
                                        inOutRecord.Type = Convert.ToInt32(INOUTRECORD_FLAG.出库);
                                        inOutRecord.Warehouse = warehouseMaterial.Warehouse;
                                        inOutRecord.WarehouseId = warehouseMaterial.WarehouseId.Value;
                                        //inOutRecord.增加
                                        addInOutRecords.Add(inOutRecord);

                                        //需要更新的location增加；
                                        Location location = locations.Find(i => i.SysCode == warehouseMaterial.Location.SysCode);
                                        location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                        location.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                                        uplocations.Add(location);
                                        // 锁定目标点

                                        batchCount -= warehouseMaterial.MaterialCount;
                                        if (batchCount <= 0)
                                            break;


                                    }

                                }                             
                            }
                            else
                            {
                                //TODO：  空托盘出库 
                              //1.查找空托盘
                                WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, null,
                                                                  new List<int> { 0,0}, null, null, null, null,
                                                                  null, null, null, areaId,null);
                                //1.1得到所有该区域下的空托盘的库存
                                List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);

                                //如果库存的空托盘小于目标值，报错
                                if (batchCount > warehouseTrays.Count)
                                {
                                    throw new Exception(string.Format("空托盘出库报错。当前区域编号{0}下空托盘数量小于需求值，剩余量为{1}，请增加空托盘的库存。",areaId, warehouseTrays.Count));

                                }

                                //按照入库完成的时间排序库位
                                warehouseTrays.OrderBy(t => t.CreateTime);

                               


                                //2.查找lccation
                                //实际上我觉得再这里把所有的饿lcaation找到，然后形成集合，再foreach中通过find的方法应该更好一点。
                                //1.更改locations
                                LocationSpecification locationSpec = new LocationSpecification(null, null, null, null, null, null,null,
                                                                                                        areaId,
                                                                                                        new List<int> { Convert.ToInt32(LOCATION_STATUS.正常) },
                                                                                                        new List<int> { Convert.ToInt32(LOCATION_INSTOCK.空托盘) },
                                                                                                        new List<int> { Convert.ToInt32(LOCATION_TASK.没有任务) },
                                                                                                        null, null, null);
                                //查找当前所有的站点信息
                                List<Location> locations = await this._locationRepository.ListAsync(locationSpec);



                                //确定出库位置
                                WarehouseTray tarWareHouseTray = warehouseTrays[0];
                                int wlckbh = tarWareHouseTray.Location.PhyWarehouseId.Value;
                                Location ckLocation = locations.Find(l => l.PhyWarehouseId == wlckbh && l.Type == Convert.ToInt32(LOCATION_TYPE.出库区货位));
                                //获得出库期望目标点
                                string tarId = ckLocation.SysCode;


                                foreach (var warehouseTray in warehouseTrays)
                                {
                                    string locationCode = warehouseTray.Location.SysCode;

                                    //1.更改locations
                                    #region 第二种方式 查找locations
                                    //LocationSpecification locationSpec = new LocationSpecification(null, locationCode, null, null, null, null,
                                    //                                                                        areaId,
                                    //                                                                        new List<int> { Convert.ToInt32(LOCATION_STATUS.正常) },
                                    //                                                                        new List<int> { Convert.ToInt32(LOCATION_INSTOCK.空托盘) },
                                    //                                                                        new List<int> { Convert.ToInt32(LOCATION_TASK.没有任务) },
                                    //                                                                        null, null, null);
                                    ////查找当前所有的站点信息
                                    //List<Location> locations = await this._locationRepository.ListAsync(locationSpec);
                                    //Location upLocation = locations[0];
                                    #endregion
                                    Location upLocation = locations.Find(n => n.SysCode == locationCode);
                                    upLocation.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                    upLocation.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                                    uplocations.Add(upLocation);


                                    //1.2更改warehouse
                                    warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.待出库);                                   
                                    warehouseTray.Location.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                                    warehouseTray.Location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                                    updTrays.Add(warehouseTray);

                                    //1.3更改inoutTask
                                    InOutTask inOutTask = new InOutTask();
                                    inOutTask.TargetId = tarId;//目标点
                                    inOutTask.SrcId = warehouseTray.Location.SysCode;//起始点
                                    inOutTask.CreateTime = DateTime.Now;//创建时间
                                    inOutTask.Type = Convert.ToInt32(TASK_TYPE.物料出库);                                   
                                    inOutTask.TrayCode = warehouseTray.TrayCode;
                                    inOutTask.Status = Convert.ToInt32(TASK_STATUS.待处理);
                                    addTasks.Add(inOutTask);


                                    batchCount--;
                                    if (batchCount == 0) break;

                                }


                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        //需要增加的orderRowBatch
                        orb.IsRead = Convert.ToInt32(ORDER_BATCH_READ.已读);
                        upOrderRows.Add(orb);


                        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            try
                            {
                                this._orderRowBathRepository.Update(upOrderRows);
                                this._locationRepository.Update(uplocations);
                                this._warehouseTrayRepository.Update(updTrays);
                                this._inOutTaskRepository.Add(addTasks);
                                this._inOutRecordRepository.Add(addInOutRecords);
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
                            upOrderRows.Clear();
                            upReservoirAreas.Clear();
                            upWarehouseMaterials.Clear();
                            addInOutRecords.Clear();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }


                    });




                }
                catch (Exception ex)
                {
                    // todo 添加异常日志记录
                    //写道数据库里面
                    
                }
            }
        }
    }
}
