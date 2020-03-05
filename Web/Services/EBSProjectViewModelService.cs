using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using Web.ViewModels.OrderManager;
using Web.ViewModels.TaskManager;

namespace Web.Services
{
    public class EBSProjectViewModelService:IEBSProjectViewModelService
    {

        private readonly IAsyncRepository<EBSProject> _ebsProjectRepository;
        public EBSProjectViewModelService(IAsyncRepository<EBSProject> ebsProjectRepository)
        {
            this._ebsProjectRepository = ebsProjectRepository;
        }

        public async Task<ResponseResultViewModel> GetProjects(int? pageIndex, int? itemsPage, int? id, string projectName,
            int? ouId, string sCreateTime,string eCreateTime, string sEndTime, string eEndTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };

            try
            {
                BaseSpecification<EBSProject> spec = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    spec = new EBSProjectPaginatedSpecification(pageIndex.Value,itemsPage.Value,id,projectName,
                        ouId,sCreateTime,eCreateTime,sEndTime,eEndTime);
                }
                else
                {
                    spec = new EBSProjectSpecification(id,projectName,ouId,sCreateTime,eCreateTime,sEndTime,eEndTime);
                }
                List<EBSProjectViewModel> ebsProjectViewModels = new List<EBSProjectViewModel>();
                var projects = await this._ebsProjectRepository.ListAsync(spec);

                projects.ForEach(e =>
                {
                    EBSProjectViewModel ebsProjectViewModel = new EBSProjectViewModel
                    {
                        Id = e.Id,
                        ProjectCode = e.ProjectCode,
                        ProjectName = e.ProjectName,
                        ProjectFullName = e.ProjectFullName,
                        OUId = e.OUId,
                        OUName = e.OU?.OUName,
                        OrgName = e.Organization?.OrgName,
                        OrganizationId = e.OrganizationId,
                        TypeCode = e.TypeCode,
                        StatusCode = e.StatusCode,
                        ProjectCategory = e.ProjectCategory,
                        EmployeeId = e.EmployeeId,
                        EmployeeName = e.Employee?.UserName,
                        CreateTime = e.CreateTime?.ToString(),
                        EndTime = e.EndTime?.ToString()
                    };
                    ebsProjectViewModels.Add(ebsProjectViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._ebsProjectRepository.CountAsync(new EBSProjectSpecification(id,
                        projectName,ouId,sCreateTime,eCreateTime,sEndTime,eEndTime));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = ebsProjectViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = ebsProjectViewModels;
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