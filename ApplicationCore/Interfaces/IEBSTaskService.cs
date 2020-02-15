using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IEBSTaskService
    {
        Task AddEBSTask(EBSTask ebsTask,bool unique=false);
        Task UpdateEBSTask(EBSTask ebsTask);

        Task AddEBSTask(List<EBSTask> ebsTasks,bool unique=false);
        Task UpdateEBSTask(List<EBSTask> ebsTasks);
    }
}