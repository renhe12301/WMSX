using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class ReservoirAreaService:IReservoirAreaService
    {
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<Location> _locationRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        public ReservoirAreaService(IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                                    IAsyncRepository<Location> locationRepository,
                                    IAsyncRepository<LogRecord> logRecordRepository
                                    )
        {
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._locationRepository = locationRepository;
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddArea(ReservoirArea reservoirArea,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(reservoirArea, nameof(reservoirArea));
                    Guard.Against.Zero(reservoirArea.Id, nameof(reservoirArea.Id));
                    Guard.Against.Zero(reservoirArea.WarehouseId, nameof(reservoirArea.WarehouseId));
                    Guard.Against.NullOrEmpty(reservoirArea.AreaCode, nameof(reservoirArea.AreaCode));
                    if (unique)
                    {
                        ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(reservoirArea.Id,
                            null,
                            null,
                            null, null, null);
                        List<ReservoirArea> areas = this._reservoirAreaRepository.List(reservoirAreaSpec);
                        if (areas.Count == 0)
                            this._reservoirAreaRepository.Add(reservoirArea);
                    }
                    else
                    {
                        this._reservoirAreaRepository.AddAsync(reservoirArea);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增库区!"),
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

        public async Task UpdateArea(List<ReservoirArea> reservoirAreas)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(reservoirAreas, nameof(reservoirAreas));
                    this._reservoirAreaRepository.Update(reservoirAreas);
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新库区集合!"),
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

        public async Task AssignLocation(int areaId, List<int> locationIds)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Zero(areaId, nameof(areaId));
                    Guard.Against.NullOrEmpty(locationIds, nameof(locationIds));
                    ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(areaId, null,
                        null, null, null, null);
                    var ares =  this._reservoirAreaRepository.List(reservoirAreaSpec);
                    Guard.Against.NullOrEmpty(ares, nameof(ares));
                    LocationSpecification locationSpecification = new LocationSpecification(null, null, null, null,
                        null, null, null, areaId, null, null, null, null, null, null);
                    var areaLocations = this._locationRepository.List(locationSpecification);
                    foreach (var areaLocation in areaLocations)
                    {
                        areaLocation.ReservoirAreaId = null;
                        areaLocation.OUId = null;
                        areaLocation.WarehouseId = null;
                    }

                    this._locationRepository.Update(areaLocations);

                    var locations =  this._locationRepository.ListAll();
                    List<Location> updLocations = new List<Location>();
                    locationIds.ForEach(async (id) =>
                    {
                        var location = locations.Find(l => l.Id == id);
                        location.ReservoirAreaId = areaId;
                        location.OUId = ares[0].OUId;
                        location.WarehouseId = ares[0].WarehouseId;
                        updLocations.Add(location);
                    });
                    this._locationRepository.Update(updLocations);
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("库区[{0}],分配货位!",areaId),
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

        public async Task UpdateArea(ReservoirArea reservoirArea)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(reservoirArea, nameof(reservoirArea));
                    this._reservoirAreaRepository.Update(reservoirArea);
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("库区[{0}],更新!",reservoirArea.Id),
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

        public async Task AddArea(List<ReservoirArea> reservoirAreas, bool unique = false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.NullOrEmpty(reservoirAreas, nameof(reservoirAreas));
                    if (unique)
                    {
                        List<ReservoirArea> adds = new List<ReservoirArea>();
                        reservoirAreas.ForEach((area) =>
                        {
                            ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(area.Id, null,
                                null,
                                null, null, null);
                            List<ReservoirArea> areas =  this._reservoirAreaRepository.List(reservoirAreaSpec);
                            if (areas.Count == 0)
                                adds.Add(area);
                        });
                        if (adds.Count > 0)
                             this._reservoirAreaRepository.Add(adds);
                    }
                    else
                    {
                         this._reservoirAreaRepository.Add(reservoirAreas);
                    }
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增库区!"),
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
