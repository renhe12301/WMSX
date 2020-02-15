using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IEBSProjectService
    {
        Task AddEBSProject(EBSProject ebsProject,bool unique=false);
        Task UpdateEBSProject(EBSProject ebsProject);

        Task AddEBSProject(List<EBSProject> ebsProjects,bool unique=false);
        Task UpdateEBSProject(List<EBSProject> ebsProjects);
    }
}