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
                                             int? id,string trayCode ,int? subOrderId,int? subOrderRowId, 
                                             string status,string steps,string types, int? ouId,
                                              int? wareHouseId, int? areaId,int? pyId,
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
                        id,trayCode,subOrderId,subOrderRowId,taskStatus, taskSteps,taskTypes,null,ouId, wareHouseId,areaId,pyId,
                        sCreateTime,eCreateTIme,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new InOutTaskSpecification(id, trayCode,subOrderId,subOrderRowId,
                        taskStatus,taskSteps,taskTypes,null, ouId, wareHouseId, areaId,pyId,
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
                        MaterialCount = e.WarehouseTray?.MaterialCount,
                        SrcId=e.SrcId,
                        TargetId=e.TargetId,
                        StatusStr= Enum.GetName(typeof(TASK_STATUS), e.Status),
                        StepStr = Enum.GetName(typeof(TASK_STEP), e.Step),
                        Type =  Enum.GetName(typeof(TASK_TYPE), e.Type),
                        OUName = e.OU?.OUName,
                        ReservoirAreaName = e.ReservoirArea?.AreaName,
                        WarehouseName = e.Warehouse?.WhName,
                        OrderId = e.SubOrderId,
                        OrderRowId = e.SubOrderRowId,
                        Progress=e.Progress,
                        FinishTime=e.FinishTime?.ToString()
                    };
                    inOutTaskViewModels.Add(inOutTaskViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._inOutTaskRepository.CountAsync(new InOutTaskSpecification(id, trayCode,
                                           subOrderId,subOrderRowId,taskStatus,taskSteps,taskTypes,null, ouId, wareHouseId, areaId,pyId,
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

        public async Task<ResponseResultViewModel> OutConfirm(WarehouseTrayViewModel warehouseTrayViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._inOutTaskService.OutConfirm(warehouseTrayViewModel.TrayCode);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }

            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> EmptyOut(WarehouseTrayViewModel warehouseTrayViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._inOutTaskService.EmptyOut(warehouseTrayViewModel.ReservoirId,
                    warehouseTrayViewModel.OutCount);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }

            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> EmptyEntry(WarehouseTrayViewModel warehouseTrayViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._inOutTaskService.EmptyEntry(warehouseTrayViewModel.TrayCode,warehouseTrayViewModel.ReservoirId);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }

            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> TaskReport(InOutTaskViewModel inOutTaskViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._inOutTaskService.TaskReport(inOutTaskViewModel.Id,0,
                    inOutTaskViewModel.Step,inOutTaskViewModel.Memo);
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
