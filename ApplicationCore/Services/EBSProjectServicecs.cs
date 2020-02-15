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
    public class EBSProjectServicecs:IEBSProjectService
    {
        private readonly IAsyncRepository<EBSProject> _ebsProjectRepository;

        public EBSProjectServicecs(IAsyncRepository<EBSProject> ebsProjectRepository)
        {
            this._ebsProjectRepository = ebsProjectRepository;
        }

        public async Task AddEBSProject(EBSProject ebsProject,bool unique=false)
        {
            Guard.Against.Null(ebsProject, nameof(ebsProject));
            Guard.Against.Zero(ebsProject.Id, nameof(ebsProject.Id));
            if (unique)
            {
                EBSProjectSpecification ebsProjectSpec = new EBSProjectSpecification(ebsProject.Id, null,
                    null,null,null,null,null);
                var projects = await this._ebsProjectRepository.ListAsync(ebsProjectSpec);
                if (projects.Count == 0)
                    await this._ebsProjectRepository.AddAsync(ebsProject);
            }
            else
            {
                await this._ebsProjectRepository.AddAsync(ebsProject);
            }
        }

        public async Task UpdateEBSProject(EBSProject ebsProject)
        {
            Guard.Against.Null(ebsProject, nameof(ebsProject));
            await  this._ebsProjectRepository.UpdateAsync(ebsProject);
        }

        public async Task AddEBSProject(List<EBSProject> ebsProjects,bool unique=false)
        {
            Guard.Against.Null(ebsProjects, nameof(ebsProjects));
            Guard.Against.NullOrEmpty(ebsProjects,nameof(ebsProjects));
            if (unique)
            {
                List<EBSProject> adds=new List<EBSProject>();
                ebsProjects.ForEach(async(em) =>
                {
                    EBSProjectSpecification ebsProjectSpec=new EBSProjectSpecification(em.Id, null,
                        null,null,null,null,null);
                    var findProjects = await this._ebsProjectRepository.ListAsync(ebsProjectSpec);
                    if(findProjects.Count>0)
                        adds.Add(findProjects.First());
                });
                if (adds.Count > 0)
                    await this._ebsProjectRepository.AddAsync(adds);
            }
            else
            {
                await this._ebsProjectRepository.AddAsync(ebsProjects);
            }
        }

        public async Task UpdateEBSProject(List<EBSProject> ebsProjects)
        {
            Guard.Against.Null(ebsProjects, nameof(ebsProjects));
            await this._ebsProjectRepository.UpdateAsync(ebsProjects);
        }
        
    }
}
