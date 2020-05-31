using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using ApplicationCore.Entities.BasicInformation;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Misc;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class MaterialDicService:IMaterialDicService
    {
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;

        public MaterialDicService(IAsyncRepository<MaterialDic> materialDicRepository,
            IAsyncRepository<LogRecord> logRecordRepository)
        {
            this._materialDicRepository = materialDicRepository;
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddMaterialDic(MaterialDic materialDic,bool unique=false)
        {

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(materialDic, nameof(materialDic));
                    Guard.Against.Zero(materialDic.Id, nameof(materialDic.Id));
                    Guard.Against.NullOrEmpty(materialDic.MaterialName,
                        nameof(materialDic.MaterialName));
                    if (unique)
                    {
                        MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(materialDic.Id, null,
                            null, null, null);
                        var materialDics =  this._materialDicRepository.List(materialDicSpec);
                        if (materialDics.Count == 0)
                             this._materialDicRepository.Add(materialDic);
                    }
                    else
                    {
                         this._materialDicRepository.Add(materialDic);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("添加物料字典!"),
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

        public async Task UpdateMaterialDic(MaterialDic materialDic)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(materialDic, nameof(materialDic));
                    this._materialDicRepository.Update(materialDic);
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新物料字典!"),
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

        public async Task AddMaterialDic(List<MaterialDic> materialDics, bool unique = false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(materialDics, nameof(materialDics));
                    if (unique)
                    {
                        List<MaterialDic> adds = new List<MaterialDic>();
                        materialDics.ForEach(async (m) =>
                        {

                            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(m.Id, null,
                                null, null, null);
                            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
                            if (materialDics.Count == 0)
                                adds.Add(materialDics.First());
                        });
                        if (adds.Count > 0)
                            await this._materialDicRepository.AddAsync(adds);
                    }
                    else
                    {
                        await this._materialDicRepository.AddAsync(materialDics);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增物料字典!"),
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

        public async Task UpdateMaterialDic(List<MaterialDic> materialDics)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(materialDics, nameof(materialDics));
                    Guard.Against.NullOrEmpty(materialDics, nameof(materialDics));
                    this._materialDicRepository.Update(materialDics);

                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新物料字典!"),
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
