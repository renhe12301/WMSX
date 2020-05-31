using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Security;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Misc;

namespace ApplicationCore.Services
{
    public class MaterialTypeService : IMaterialTypeService
    {

        private readonly IAsyncRepository<MaterialType> _materialTypeRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        public MaterialTypeService(IAsyncRepository<MaterialType> materialTypeRepository,
                                   IAsyncRepository<LogRecord> logRecordRepository
            )
        {
            this._materialTypeRepository = materialTypeRepository;
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddMaterialType(MaterialType materialType,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(materialType, nameof(materialType));
                    if (unique)
                    {
                        var materialTypeSpec = new MaterialTypeSpecification(null, null, materialType.TypeName);
                        var materialTypes = await this._materialTypeRepository.ListAsync(materialTypeSpec);
                        if (materialTypes.Count == 0)
                             this._materialTypeRepository.Add(materialType);
                    }
                    else
                    {
                         this._materialTypeRepository.Add(materialType);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增物料字典类型!"),
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

        public async Task AddMaterialType(List<MaterialType> materialTypes,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(materialTypes, nameof(materialTypes));
                    if (unique)
                    {
                        List<MaterialType> adds = new List<MaterialType>();
                        materialTypes.ForEach(async (mt) =>
                        {
                            var materialTypeSpec = new MaterialTypeSpecification(null, null, mt.TypeName);
                            var mts =  this._materialTypeRepository.List(materialTypeSpec);
                            if (mts.Count == 0)
                                adds.Add(mts.First());
                        });
                        if (adds.Count > 0)
                             this._materialTypeRepository.Add(adds);
                    }
                    else
                    {
                         this._materialTypeRepository.Add(materialTypes);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增物料字典类型!"),
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

        public async Task UpdateMaterialType(MaterialType materialType)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(materialType, nameof(materialType));
                    this._materialTypeRepository.Update(materialType);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新物料字典类型!"),
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

        public async Task UpdateMaterialType(List<MaterialType> materialTypes)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(materialTypes, nameof(materialTypes));
                    this._materialTypeRepository.Update(materialTypes);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新物料字典类型!"),
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
