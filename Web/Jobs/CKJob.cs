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
    /// <summary>
    /// 出库订单处理定时任务
    /// </summary>
    public class CkJob:IJob
    {
       

        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<Location> _locationRepository;

        public CkJob(IAsyncRepository<WarehouseTray> warehouseTrayRepository,
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
            
        }
    }
}
