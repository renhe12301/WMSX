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
    public class OrganizationService : IOrganizationService
    {
        private readonly IAsyncRepository<Organization> _organizationRepository;
        private readonly IAsyncRepository<LogRecord> _logRecordRepository;
        

        public OrganizationService(IAsyncRepository<Organization> organizationRepository,
            IAsyncRepository<LogRecord> logRecordRepository)
        {
            this._organizationRepository = organizationRepository;
            this._logRecordRepository = logRecordRepository;
        }

        public async Task AddOrg(Organization org,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(org, nameof(org));
                    Guard.Against.Zero(org.Id, nameof(org.Id));
                    Guard.Against.NullOrEmpty(org.OrgCode, nameof(org.OrgCode));
                    Guard.Against.NullOrEmpty(org.OrgName, nameof(org.OrgName));
                    if (unique)
                    {
                        OrganizationSpecification organizationSpec =
                            new OrganizationSpecification(org.Id, null, null, null);
                        List<Organization> orgs =  this._organizationRepository.List(organizationSpec);
                        if (orgs.Count == 0)
                             this._organizationRepository.Add(org);
                    }
                    else
                    {
                         this._organizationRepository.Add(org);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增部门!"),
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

        public async Task UpdateOrg(Organization srcOrg)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(srcOrg, nameof(srcOrg));
                    this._organizationRepository.Update(srcOrg);
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新部门!"),
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

        public async Task AddOrg(List<Organization> orgs,bool unique=false)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(orgs, nameof(orgs));
                    if (unique)
                    {
                        List<Organization> adds = new List<Organization>();
                        orgs.ForEach(async (o) =>
                        {
                            OrganizationSpecification organizationSpec =
                                new OrganizationSpecification(o.Id, null, null, null);
                            List<Organization> orgs = await this._organizationRepository.ListAsync(organizationSpec);
                            if (orgs.Count == 0)
                                adds.Add(o);
                        });

                        if (adds.Count > 0)
                             this._organizationRepository.Add(adds);
                    }
                    else
                    {
                         this._organizationRepository.Add(orgs);
                    }
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("新增部门!"),
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

        public async Task UpdateOrg(List<Organization> orgs)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Guard.Against.Null(orgs, nameof(orgs));
                    this._organizationRepository.Update(orgs);
                    
                    this._logRecordRepository.Add(new LogRecord
                    {
                        LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                        LogDesc = string.Format("更新部门!"),
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
