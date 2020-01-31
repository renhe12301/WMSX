using System;
using System.Threading.Tasks;
using Quartz;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using ApplicationCore.Misc;
using ApplicationCore.Entities.BasicInformation;
using System.Linq;

namespace Web.Jobs
{
    public class CK_Job:IJob
    {
       

        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<Location> _locationRepository;

        public CK_Job(IAsyncRepository<WarehouseTray> warehouseTrayRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<ReservoirArea> reservoirAreaRepository,
            IAsyncRepository<Location> locationRepository)
        {
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._warehouseRepository = warehouseRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._locationRepository = locationRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            
            try
            {
                WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, null,
                    null, null, null, null,null, new List<int>
                    {
                        Convert.ToInt32(TRAY_STEP.待出库)
                    }
                    ,null
                    ,null
                    ,null
                    ,null
                    ,null
                    );
                var awaitTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                var groupTrays = awaitTrays.GroupBy(g => g.WarehouseId);
                foreach (var gTray in groupTrays)
                {
                    try
                    {
                        var wid = gTray.Key;
                        WarehouseSpecification warehouseSpec = new WarehouseSpecification(wid, null, null, null);
                        var warehouses = await this._warehouseRepository.ListAsync(warehouseSpec);
                        if (warehouses.Count > 0 && !string.IsNullOrEmpty(warehouses[0].Organization.Memo))
                        {
                            string weburl = warehouses[0].Memo;
                           
                            LocationSpecification locationSpec = new LocationSpecification(null, null,null,null,
                                                                                     null, null, null,
                                new List<int>{Convert.ToInt32(LOCATION_STATUS.正常)},
                                new List<int>{Convert.ToInt32(LOCATION_INSTOCK.无货)},null,null,null,null);
                            var locations = await this._locationRepository.ListAsync(locationSpec);
                            int index = 0;
                            //发送wcs任务
                            foreach (var warehouseTray in gTray.ToList())
                            {
                                if (index + 1 >= locations.Count) index = 0;
                                var location = locations[index];
                                string srcId = warehouseTray.Location.SysCode;
                                string targetId = location.SysCode;
                                
                                //todo 发送wcs任务

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // todo 添加异常日志记录
                    }
                   
                }

            }
            catch (Exception ex)
            {
                // todo 添加异常日志记录
            }
        }
    }
}
