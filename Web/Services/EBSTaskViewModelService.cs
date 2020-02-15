using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Services
{
    public class EBSTaskViewModelService:IEBSTaskViewModelService
    {

        private readonly IAsyncRepository<EBSTask> _ebsTaskRepository;

        public EBSTaskViewModelService(IAsyncRepository<EBSTask> ebsTaskRepository)
        {
            this._ebsTaskRepository = ebsTaskRepository;
        }

        public async Task<ResponseResultViewModel> GetTasks(int? pageIndex, int? itemsPage, int? id, string taskName, 
            int? projectId, string sCreateTime,string eCreateTime, string sEndTime, string eEndTime)
        {
             ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };

            try
            {
                BaseSpecification<EBSTask> spec = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    spec = new EBSTaskPaginatedSpecification(pageIndex.Value,itemsPage.Value,id,taskName,projectId,
                        sCreateTime,sEndTime,eCreateTime,eCreateTime);
                }
                else
                {
                    spec = new EBSTaskSpecification(id,taskName,projectId,
                        sCreateTime,sEndTime,eCreateTime,eCreateTime);
                }
                List<EBSTaskViewModel> ebsTaskViewModels = new List<EBSTaskViewModel>();
                var tasks = await this._ebsTaskRepository.ListAsync(spec);

                tasks.ForEach(e =>
                {
                    EBSTaskViewModel ebsTaskViewModel = new EBSTaskViewModel
                    {
                        Id = e.Id,
                        TaskCode = e.TaskCode,
                        TaskName = e.TaskName,
                        Summary = e.Summary,
                        TaskLevel = e.TaskLevel,
                        ProjectId = e.EBSProjectId,
                        ProjectName = e.EBSProject?.ProjectName,
                        CreateTime = e.CreateTime?.ToString(),
                        EndTime = e.EndTime?.ToString()
                    };
                    ebsTaskViewModels.Add(ebsTaskViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._ebsTaskRepository.CountAsync(new EBSTaskSpecification(id,taskName,projectId,
                        sCreateTime,sEndTime,eCreateTime,eCreateTime));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = ebsTaskViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = ebsTaskViewModels;
                }
                
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
    }
}