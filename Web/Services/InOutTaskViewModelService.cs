using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.TaskManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Misc;
using Web.ViewModels.StockManager;

namespace Web.Services
{
    public class InOutTaskViewModelService:IInOutTaskViewModelService
    {

        private readonly IInOutTaskService _inOutTaskService;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;

        public InOutTaskViewModelService(IInOutTaskService inOutTaskService,
                                         IAsyncRepository<InOutTask> inOutTaskRepository)
        {
            this._inOutTaskService = inOutTaskService;
            this._inOutTaskRepository = inOutTaskRepository;
        }


        public async Task<ResponseResultViewModel> GetInOutTasks(int? pageIndex, int? itemsPage,
                                             int? id, string status,string steps,
                                              string sCreateTime, string eCreateTIme,
                                              string sFinishTime, string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<InOutTask> baseSpecification = null;
                List<int> taskStatus = null;
                if (!string.IsNullOrEmpty(status))
                {
                    taskStatus = status.Split(new char[]{
               ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                List<int> taskSteps = null;
                if (!string.IsNullOrEmpty(steps))
                {
                    taskSteps = steps.Split(new char[]{
               ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new InOutTaskPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,taskStatus, taskSteps, sCreateTime,eCreateTIme,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new InOutTaskSpecification(id, 
                        taskStatus,taskSteps, sCreateTime, eCreateTIme, sFinishTime, eFinishTime);
                }

                var tasks = await this._inOutTaskRepository.ListAsync(baseSpecification);
                List<InOutTaskViewModel> inOutTaskViewModels = new List<InOutTaskViewModel>();

                tasks.ForEach(e =>
                {
                    InOutTaskViewModel inOutTaskViewModel = new InOutTaskViewModel
                    {
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        Memo = e.Memo,
                        WarehouseTrayCode=e.WarehouseTray.Code,
                        SrcId=e.SrcId,
                        TargetId=e.TargetId,
                        StatusStr= Enum.GetName(typeof(TASK_STATUS), e.Status),
                        StepStr = Enum.GetName(typeof(TASK_STEP), e.Step),
                        Progress=e.Progress,
                        FinishTime=e.FinishTime.ToString()
                    };
                    inOutTaskViewModels.Add(inOutTaskViewModel);
                });
                response.Data = inOutTaskViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> EmptyAwaitInApply(WarehouseTrayViewModel warehouseTrayViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._inOutTaskService.EmptyAwaitInApply(warehouseTrayViewModel.Code,
                                             warehouseTrayViewModel.WarehouseId,warehouseTrayViewModel.ReservoirId);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> AwaitOutApply(InOutTaskViewModel inOutTaskViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                if (!inOutTaskViewModel.WarehouseId.HasValue) throw new Exception("仓库编号不能为空！");
                await this._inOutTaskService.AwaitOutApply(inOutTaskViewModel.OrderId,inOutTaskViewModel.OrderRowId
                    ,inOutTaskViewModel.TrayCodes);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> InApply(InOutTaskViewModel inOutTaskViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                if (!inOutTaskViewModel.WarehouseId.HasValue) throw new Exception("仓库编号不能为空！");
                await this._inOutTaskService.InApply(inOutTaskViewModel.WarehouseTrayCode,inOutTaskViewModel.LocationCode);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> TaskStepReport(InOutTaskViewModel inOutTaskViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                if (!inOutTaskViewModel.WarehouseId.HasValue) throw new Exception("仓库编号不能为空！");
                await this._inOutTaskService.TaskStepReport(inOutTaskViewModel.Id,
                    inOutTaskViewModel.VehicleId, inOutTaskViewModel.VehicleName, inOutTaskViewModel.Step);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }
    }
}
