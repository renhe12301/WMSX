using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.TaskManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ApplicationCore.Misc;
using Web.ViewModels.StockManager;
using ApplicationCore.Entities.StockManager;

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
                                             int? id,string trayCode , string status,string steps,string types,
                                              int? orgId, int? ouId,
                                              int? wareHouseId, int? areaId,
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
                List<int> taskTypes = null;
                if (!string.IsNullOrEmpty(types))
                {
                    taskTypes = types.Split(new char[]{
                        ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new InOutTaskPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,trayCode,taskStatus, taskSteps,taskTypes,orgId,ouId, wareHouseId,areaId, 
                        sCreateTime,eCreateTIme,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new InOutTaskSpecification(id, trayCode,
                        taskStatus,taskSteps,taskTypes, orgId, ouId, wareHouseId, areaId, 
                        sCreateTime, eCreateTIme, sFinishTime, eFinishTime);
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
                        TrayCode = e.TrayCode,
                        SrcId=e.SrcId,
                        TargetId=e.TargetId,
                        StatusStr= Enum.GetName(typeof(TASK_STATUS), e.Status),
                        StepStr = Enum.GetName(typeof(TASK_STEP), e.Step),
                        Type =  Enum.GetName(typeof(TASK_Type), e.Type),
                        OUName = e.OU?.OUName,
                        ReservoirAreaName = e.ReservoirArea?.AreaName,
                        WarehouseName = e.Warehouse?.WhName,
                        OrgName = e.Organization?.OrgName,
                        OrderId = e.OrderId,
                        OrderRowId = e.OrderRowId,
                        Progress=e.Progress,
                        FinishTime=e.FinishTime?.ToString()
                    };
                    inOutTaskViewModels.Add(inOutTaskViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._inOutTaskRepository.CountAsync(new InOutTaskSpecification(id, trayCode,
                                           taskStatus,taskSteps,taskTypes, orgId, ouId, wareHouseId, areaId, 
                                           sCreateTime, eCreateTIme, sFinishTime, eFinishTime));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = inOutTaskViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = inOutTaskViewModels;
                }
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
                                             warehouseTrayViewModel.WarehouseId);
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
                await this._inOutTaskService.AwaitOutApply(inOutTaskViewModel.OrderId.Value,
                    inOutTaskViewModel.OrderRowId.Value,inOutTaskViewModel.WarehouseTrays);
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
                await this._inOutTaskService.InApply(inOutTaskViewModel.TrayCode,inOutTaskViewModel.LocationCode);
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
                    inOutTaskViewModel.VehicleId.GetValueOrDefault(0), inOutTaskViewModel.Step);
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
