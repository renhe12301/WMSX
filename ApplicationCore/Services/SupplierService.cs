using System;
using System.Collections.Generic;
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
    public class SupplierService:ISupplierService
    {
        private readonly IAsyncRepository<Supplier> _supplierRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;

        public SupplierService(IAsyncRepository<Supplier> supplierRepository,
            IAsyncRepository<LogRecord> logRecordRepository
            )
        {
            this._supplierRepository = supplierRepository;
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddSupplier(Supplier supplier,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(supplier, nameof(supplier));
                    Guard.Against.Zero(supplier.Id, nameof(supplier.Id));
                    Guard.Against.NullOrEmpty(supplier.SupplierCode, nameof(supplier.SupplierCode));
                    Guard.Against.NullOrEmpty(supplier.SupplierName, nameof(supplier.SupplierName));
                    if (unique)
                    {
                        SupplierSpecification supplierSpec = new SupplierSpecification(supplier.Id, null);
                        List<Supplier> suppliers =  this._supplierRepository.List(supplierSpec);
                        if (suppliers.Count == 0)
                             this._supplierRepository.Add(supplier);
                    }
                    else
                    {
                         this._supplierRepository.Add(supplier);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增供应商!"),
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

        public async Task AddSupplier(List<Supplier> suppliers, bool unique = false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(suppliers, nameof(suppliers));
                    if (unique)
                    {
                        List<Supplier> adds = new List<Supplier>();
                        suppliers.ForEach(async (s) =>
                        {
                            SupplierSpecification supplierSpec = new SupplierSpecification(s.Id, null);
                            List<Supplier> suppliers =  this._supplierRepository.List(supplierSpec);
                            if (suppliers.Count == 0)
                                adds.Add(s);
                        });
                        if (adds.Count > 0)
                            this._supplierRepository.Add(adds);
                    }
                    else
                    {
                        this._supplierRepository.Add(suppliers);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增供应商!"),
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

        public async Task UpdateSupplier(Supplier supplier)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(supplier, nameof(supplier));
                    this._supplierRepository.Update(supplier);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新供应商!"),
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

        public async Task UpdateSupplier(List<Supplier> suppliers)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(suppliers, nameof(suppliers));
                     this._supplierRepository.Update(suppliers);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新供应商!"),
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
