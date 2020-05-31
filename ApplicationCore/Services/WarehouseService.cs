using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    public class WarehouseService:IWarehouseService
    {
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        public WarehouseService(IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<LogRecord> logRecordRepository
            )
        {
            this._warehouseRepository = warehouseRepository;
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddWarehouse(Warehouse warehouse,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(warehouse, nameof(warehouse));
                    Guard.Against.Zero(warehouse.Id, nameof(warehouse.Id));
                    Guard.Against.NullOrEmpty(warehouse.WhName, nameof(warehouse.WhName));
                    Guard.Against.NullOrEmpty(warehouse.WhCode, nameof(warehouse.WhCode));
                    if (unique)
                    {
                        WarehouseSpecification warehouseSpec = new WarehouseSpecification(warehouse.Id, null, null, null);
                        List<Warehouse> warehouses =  this._warehouseRepository.List(warehouseSpec);
                        if (warehouses.Count == 0)
                            this._warehouseRepository.Add(warehouse);
                    }
                    else
                    {
                        this._warehouseRepository.Add(warehouse);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增库存组织!"),
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
        

        public async Task Disable(int id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    var wareHouseSpec = new WarehouseSpecification(id, null, null, null);
                    var wareHouses =  this._warehouseRepository.List(wareHouseSpec);
                    Guard.Against.NullOrEmpty(wareHouses, nameof(wareHouses));
                    var wareHouse = wareHouses[0];
                    wareHouse.Status = Convert.ToInt32(WAREHOUSE_STATUS.禁用);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("禁用库存组织!"),
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

        public async Task Enable(int id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    var wareHouseSpec = new WarehouseSpecification(id, null, null, null);
                    var wareHouses =  this._warehouseRepository.List(wareHouseSpec);
                    Guard.Against.NullOrEmpty(wareHouses, nameof(wareHouses));
                    var wareHouse = wareHouses[0];
                    wareHouse.Status = Convert.ToInt32(WAREHOUSE_STATUS.正常);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("启用库存组织!"),
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

        public async Task UpdateWarehouse(Warehouse warehouse)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(warehouse, nameof(warehouse));
                     this._warehouseRepository.Update(warehouse);
                     
                     this._logRecordRepository.Add(new LogRecord
                     {
                         LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                         LogDesc = string.Format("更新库存组织!"),
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

        public async Task AddWarehouse(List<Warehouse> warehouses, bool unique = false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(warehouses, nameof(warehouses));
                    if (unique)
                    {
                        List<Warehouse> adds = new List<Warehouse>();
                        warehouses.ForEach( (w) =>
                        {
                            var wareHouseSpec = new WarehouseSpecification(w.Id, null, null, null);
                            var wareHouses =  this._warehouseRepository.List(wareHouseSpec);
                            if (wareHouses.Count > 0)
                                adds.Add(wareHouses.First());
                        });
                        if (adds.Count > 0)
                             this._warehouseRepository.Add(adds);
                    }
                    else
                    {
                         this._warehouseRepository.Add(warehouses);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增库存组织!"),
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

        public async Task UpdateWarehouse(List<Warehouse> warehouses)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(warehouses, nameof(warehouses));
                    this._warehouseRepository.Update(warehouses);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新库存组织!"),
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
