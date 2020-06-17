using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class LocationService:ILocationService
    {
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        private readonly IAsyncRepository<InOutTask> _inoutTaskRepository;
        public LocationService(IAsyncRepository<Location> locationRepository,
                               IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                               IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                               IAsyncRepository<LogRecord> logRecordRepository,
                               IAsyncRepository<InOutTask> inoutTaskRepository
                               )
        {
            this._locationRepository = locationRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._logRecordRepository = logRecordRepository;
            this._inoutTaskRepository = inoutTaskRepository;
        }

        public async Task AddLocation(Location location)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(location, nameof(location));
                    Guard.Against.NullOrEmpty(location.SysCode, nameof(location.SysCode));
                    this._locationRepository.Add(location);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增货位[{0}]!",location.SysCode),
                        CreateTime = DateTime.Now
                    });
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
                
            }
        }

        public async Task BuildLocation(int phyId, int row, int rank, int col)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Zero(phyId, nameof(phyId));
                    Guard.Against.Zero(row, nameof(row));
                    Guard.Against.Zero(rank, nameof(rank));
                    Guard.Against.Zero(col, nameof(col));

                    List<Location> addLocations = new List<Location>();
                    DateTime now = DateTime.Now;
                    for (int i = 1; i <= row; i++)
                    {
                        for (int j = 1; j <= rank; j++)
                        {
                            for (int k = 1; k <= col; k++)
                            {
                                string code = phyId.ToString() + "-" + i.ToString().PadLeft(3, '0') + "-" +
                                              j.ToString().PadLeft(3, '0') + "-" +
                                              k.ToString().PadLeft(3, '0');
                                string locationCode = code;
                                Location location = new Location
                                {
                                    SysCode = locationCode,
                                    UserCode = locationCode,
                                    CreateTime = now,
                                    Floor = i,
                                    Item = j,
                                    Col = k,
                                    PhyWarehouseId = phyId,
                                    Type = Convert.ToInt32(LOCATION_TYPE.仓库区货位)
                                };
                                addLocations.Add(location);
                            }
                        }
                    }
                    addLocations.ForEach(l => { this._locationRepository.Add(l); });
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("物理仓库[{0}],生成货位[{1}]!",phyId,row.ToString()+","+rank.ToString()+","+col.ToString()),
                        CreateTime = DateTime.Now
                    });
                    
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }

            }

        }

        public async Task Clear(List<int> ids)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.NullOrEmpty(ids, nameof(ids));

                    foreach (var lid in ids)
                    {
                        LocationSpecification locationSpec = new LocationSpecification(lid,null,null,null,null,null,
                            null,null,null,null,null,null,null,null);
                        List<Location> locs = this._locationRepository.List(locationSpec);
                        if (locs.Count > 0)
                        {
                            Location loc = locs[0];
                            loc.InStock = Convert.ToInt32(LOCATION_INSTOCK.无货);
                            WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null,null,null,null,null,
                                null,null,lid,null,null,null,null);
                            List<WarehouseTray> warehouseTrays = this._warehouseTrayRepository.List(warehouseTraySpec);
                            if (warehouseTrays.Count > 0)
                            {
                                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,null,
                                    null,null,null,null,warehouseTrays[0].Id,null,null,
                                    null,null,null,null,null,null,null,null,null,null,null);
                                List<WarehouseMaterial> warehouseMaterials = this._warehouseMaterialRepository.List(warehouseMaterialSpec);
                                if (warehouseMaterials.Count > 0)
                                {
                                    this._warehouseMaterialRepository.Delete(warehouseMaterials[0]);
                                }
                                
                                InOutTaskSpecification inOutTaskSpec = new InOutTaskSpecification(null,warehouseTrays[0].TrayCode,null,null,
                                    null,null,null,null,null,null,null,null,null,
                                    null,null,null,null);
                                List<InOutTask> tasks = this._inoutTaskRepository.List(inOutTaskSpec);
                                foreach (var task in tasks)
                                {
                                    task.WarehouseTrayId = null;
                                    
                                    this._inoutTaskRepository.Update(task);
                                }
                                
                                this._warehouseTrayRepository.Delete(warehouseTrays[0]);
                            }
                            this._locationRepository.Update(loc);
                            
                        }
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
            }
            
        }

        public async Task Disable(List<int> ids)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.NullOrEmpty(ids, nameof(ids));
                    var locations =  this._locationRepository.ListAll();
                    Guard.Against.NullOrEmpty(locations, nameof(locations));

                    List<Location> updLocations = new List<Location>();
                    locations.ForEach(l =>
                    {
                        if (ids.Contains(l.Id))
                        {
                            if (l.IsTask == Convert.ToInt32(LOCATION_TASK.有任务))
                                throw new Exception(string.Format("货位[{0}]当前有任务,无法禁用！", l.SysCode));
                            l.Status = Convert.ToInt32(LOCATION_STATUS.禁用);
                            updLocations.Add(l);
                        }
                    });
                    this._locationRepository.Update(updLocations);
                    
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("禁用货位!"),
                        CreateTime = DateTime.Now
                    });
                    
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
                
            }
        }

        public async Task Enable(List<int> ids)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.NullOrEmpty(ids, nameof(ids));
                    var locations =  this._locationRepository.ListAll();
                    Guard.Against.NullOrEmpty(locations, nameof(locations));

                    List<Location> updLocations = new List<Location>();
                    locations.ForEach(l =>
                    {
                        if (ids.Contains(l.Id))
                        {
                            if (l.IsTask == Convert.ToInt32(LOCATION_TASK.有任务))
                                throw new Exception(string.Format("货位[{0}]当前有任务,无法启用！", l.SysCode));
                            l.Status = Convert.ToInt32(LOCATION_STATUS.正常);
                            updLocations.Add(l);
                        }
                    });
                    this._locationRepository.Update(updLocations);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("启用货位!"),
                        CreateTime = DateTime.Now
                    });
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
            }
        }

        public async Task UpdateLocation(int id, string sysCode, string userCode)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Zero(id, nameof(id));
                    var locationSpec = new LocationSpecification(id, null, null, null, null, null,
                        null, null, null, null, null, null, null, null);
                    var sysCodelocationSpec = new LocationSpecification(null, sysCode, null, null, null, null,
                        null, null, null, null, null, null, null, null);
                    var userCodelocationSpec = new LocationSpecification(null, null, userCode, null, null, null,
                        null, null, null, null, null, null, null, null);
                    var locations = this._locationRepository.List(locationSpec);
                    Guard.Against.NullOrEmpty(locations, nameof(locations));
                    var location = locations[0];
                    if (!string.IsNullOrEmpty(sysCode))
                    {
                        var sysCodelocations = this._locationRepository.List(sysCodelocationSpec);
                        if (sysCodelocations.Count > 0)
                        {
                            if (sysCodelocations[0].Id != id)
                                throw new Exception(string.Format("货位系统编号[{0}],已经存在！", sysCode));
                        }

                        location.SysCode = sysCode;
                    }

                    if (!string.IsNullOrEmpty(userCode))
                    {
                        var userCodelocations = this._locationRepository.List(userCodelocationSpec);
                        if (userCodelocations.Count > 0)
                        {
                            if (userCodelocations[0].Id != id)
                                throw new Exception(string.Format("货位用户编号[{0}],已经存在！", userCode));
                        }

                        location.UserCode = userCode;
                    }

                    this._locationRepository.Update(location);

                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新货位!"),
                        CreateTime = DateTime.Now
                    });

                    scope.Complete();

                }
                catch (Exception ex)
                {
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                        LogDesc = ex.ToString(),
                        CreateTime = DateTime.Now
                    });
                    throw;
                }
            }
        }
    }
}
