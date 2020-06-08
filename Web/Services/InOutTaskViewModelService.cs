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
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Misc;
using Web.ViewModels.StockManager;


namespace Web.Services
{
    public class InOutTaskViewModelService:IInOutTaskViewModelService
    {

        private readonly IInOutTaskService _inOutTaskService;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<Location> _locationRepository;

        public InOutTaskViewModelService(IInOutTaskService inOutTaskService,
                                         IAsyncRepository<InOutTask> inOutTaskRepository,
                                         IAsyncRepository<Location> locationRepository
                                         )
        {
            this._inOutTaskService = inOutTaskService;
            this._inOutTaskRepository = inOutTaskRepository;
            this._locationRepository = locationRepository;
        }


        public async Task<ResponseResultViewModel> GetInOutTasks(int? pageIndex, int? itemsPage,
                                             int? id,string trayCode ,string materialCode,int? subOrderId,int? subOrderRowId, 
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
                        id,trayCode,materialCode,subOrderId,subOrderRowId,taskStatus, taskSteps,taskTypes,null,ouId, wareHouseId,areaId,pyId,
                        sCreateTime,eCreateTIme,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new InOutTaskSpecification(id, trayCode,materialCode,subOrderId,subOrderRowId,
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
                        Status = e.Status,
                        StatusStr= Enum.GetName(typeof(TASK_STATUS), e.Status),
                        StepStr = Enum.GetName(typeof(TASK_STEP), e.Step),
                        TypeStr =  Enum.GetName(typeof(TASK_TYPE), e.Type),
                        OUId = e.OUId,
                        OUName = e.OU?.OUName,
                        ReservoirAreaId = e.ReservoirAreaId,
                        ReservoirAreaName = e.ReservoirArea?.AreaName,
                        WarehouseId = e.WarehouseId,
                        WarehouseName = e.Warehouse?.WhName,
                        SubOrderId = e.SubOrderId,
                        SubOrderRowId = e.SubOrderRowId,
                        Progress=e.Progress,
                        FinishTime=e.FinishTime?.ToString(),
                        IsRead = e.IsRead,
                        IsReadStr = Enum.GetName(typeof(TASK_READ), e.IsRead),
                        PhyWarehouseId = e.PhyWarehouseId,
                        WarehouseTrayId = e.WarehouseTrayId,
                        PhyName = e.PhyWarehouse?.PhyName
                    };
                    inOutTaskViewModels.Add(inOutTaskViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._inOutTaskRepository.CountAsync(new InOutTaskSpecification(id, trayCode,materialCode,
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

        public async Task<ResponseResultViewModel> SendWcs(InOutTaskViewModel inOutTaskViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                if(inOutTaskViewModel.SrcId==null)
                    throw new Exception(string.Format("取货货位编码不能为空!"));
                if(inOutTaskViewModel.TargetId==null)
                    throw new Exception(string.Format("取货货位编码不能为空!"));
                if(inOutTaskViewModel.PhyWarehouseId==null)
                    throw new Exception(string.Format("物理仓库编号不能为空!"));
                
                LocationSpecification srcLocationSpec = new LocationSpecification(null,inOutTaskViewModel.SrcId,null,
                    null,inOutTaskViewModel.PhyWarehouseId,null,null,null,null,null,null,
                    null,null,null);
                List<Location> srcLocations = await this._locationRepository.ListAsync(srcLocationSpec);
                
                if(srcLocations.Count==0)
                    throw new Exception(string.Format("货位编码[{0}]不存在!",inOutTaskViewModel.SrcId));
                LocationSpecification targetTocationSpec = new LocationSpecification(null,inOutTaskViewModel.TargetId,null,
                    null,inOutTaskViewModel.PhyWarehouseId,null,null,null,null,null,null,
                    null,null,null);
                List<Location> tarLocations = await this._locationRepository.ListAsync(targetTocationSpec);
                if(tarLocations.Count==0)
                    throw new Exception(string.Format("货位编码[{0}]不存在!",inOutTaskViewModel.TargetId));
              
                
                InOutTask inOutTask = new InOutTask();
                inOutTask.Type = inOutTaskViewModel.Type;
                inOutTask.SrcId = inOutTaskViewModel.SrcId;
                inOutTask.TargetId = inOutTaskViewModel.TargetId;
                inOutTask.Status = Convert.ToInt32(TASK_STATUS.待处理);
                inOutTask.IsRead = Convert.ToInt32(TASK_READ.未读);
                inOutTask.PhyWarehouseId = inOutTaskViewModel.PhyWarehouseId;
                inOutTask.Type = Convert.ToInt32(TASK_TYPE.手动下发);
                await this._inOutTaskRepository.AddAsync(inOutTask);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }

            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> TrayEntry(WarehouseTrayViewModel warehouseTrayViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._inOutTaskService.TrayEntry(warehouseTrayViewModel.TrayCode,warehouseTrayViewModel.ReservoirAreaId.GetValueOrDefault());
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }

            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> EntryApply(WarehouseTrayViewModel warehouseTrayViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._inOutTaskService.EntryApply(warehouseTrayViewModel.LocationCode,
                    warehouseTrayViewModel.TrayCode, warehouseTrayViewModel.CargoHeight.GetValueOrDefault(),warehouseTrayViewModel.CargoWeight);
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
                await this._inOutTaskService.EmptyOut(warehouseTrayViewModel.ReservoirAreaId.GetValueOrDefault(),
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
                await this._inOutTaskService.EmptyEntry(warehouseTrayViewModel.TrayCode,warehouseTrayViewModel.ReservoirAreaId.GetValueOrDefault());
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
