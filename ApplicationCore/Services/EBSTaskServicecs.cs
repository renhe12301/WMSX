using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.AuthorityManager;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using ApplicationCore.Misc;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Services
{
    public class EBSTaskServicecs:IEBSTaskService
    {
        private readonly IAsyncRepository<EBSTask> _ebsTaskRepository;

        public EBSTaskServicecs(IAsyncRepository<EBSTask> ebsTaskRepository)
        {
            this._ebsTaskRepository = ebsTaskRepository;
        }

        public async Task AddEBSTask(EBSTask ebsTask,bool unique=false)
        {
            Guard.Against.Null(ebsTask, nameof(ebsTask));
            Guard.Against.Zero(ebsTask.Id, nameof(ebsTask.Id));
            if (unique)
            {
                EBSTaskSpecification ebsTaskSpec = new EBSTaskSpecification(ebsTask.Id,null,null,
                    null,null,null,null);
                var projects = await this._ebsTaskRepository.ListAsync(ebsTaskSpec);
                if (projects.Count == 0)
                    await this._ebsTaskRepository.AddAsync(ebsTask);
            }
            else
            {
                await this._ebsTaskRepository.AddAsync(ebsTask);
            }
        }

        public async Task UpdateEBSTask(EBSTask ebsTask)
        {
            Guard.Against.Null(ebsTask, nameof(ebsTask));
            await  this._ebsTaskRepository.UpdateAsync(ebsTask);
        }

        public async Task AddEBSTask(List<EBSTask> ebsTasks,bool unique=false)
        {
            Guard.Against.Null(ebsTasks, nameof(ebsTasks));
            Guard.Against.NullOrEmpty(ebsTasks,nameof(ebsTasks));
            if (unique)
            {
                List<EBSTask> adds=new List<EBSTask>();
                ebsTasks.ForEach(async(em) =>
                {
                    EBSTaskSpecification ebsProjectSpec=new EBSTaskSpecification(em.Id, null,
                        null,null,null,null,null);
                    var findTasks = await this._ebsTaskRepository.ListAsync(ebsProjectSpec);
                    if(findTasks.Count>0)
                        adds.Add(findTasks.First());
                });
                if (adds.Count > 0)
                    await this._ebsTaskRepository.AddAsync(adds);
            }
            else
            {
                await this._ebsTaskRepository.AddAsync(ebsTasks);
            }
        }

        public async Task UpdateEBSTask(List<EBSTask> ebsTasks)
        {
            Guard.Against.Null(ebsTasks, nameof(ebsTasks));
            await this._ebsTaskRepository.UpdateAsync(ebsTasks);
        }
        
    }
}
