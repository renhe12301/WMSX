using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;
using Web.ViewModels;
using Web.Interfaces;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Specifications;
using ApplicationCore.Misc;
using Microsoft.AspNetCore.Http;

namespace Web.Services
{
    public class ReservoirAreaViewModelService:IReservoirAreaViewModelService
    {
        private readonly IReservoirAreaService _reservoirAreaService;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly ILogRecordService _logRecordService;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRepository;
        public ReservoirAreaViewModelService(IReservoirAreaService reservoirAreaService,
                                             IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                                             ILogRecordService logRecordService,
                                             IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                                             IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                                             IAsyncRepository<InOutRecord> inOutRecordRepository)
        {
            this._reservoirAreaService = reservoirAreaService;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._logRecordService = logRecordService;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._inOutRecordRepository = inOutRecordRepository;
        }


        public async Task<ResponseResultViewModel> GetAreas(int? pageIndex, int? itemsPage, int? id, 
             int? ouId, int? wareHouseId,int? type, string areaName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<ReservoirArea> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new ReservoirAreaPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,ouId,wareHouseId,areaName);
                }
                else
                {
                    baseSpecification = new ReservoirAreaSpecification(id,null,ouId,wareHouseId,type, areaName);
                }
                var areas = await this._reservoirAreaRepository.ListAsync(baseSpecification);
                List<ReservoirAreaViewModel> areaViewModels = new List<ReservoirAreaViewModel>();
                areas.ForEach(e =>
                {
                    ReservoirAreaViewModel areaViewModel = new ReservoirAreaViewModel
                    {
                        AreaName = e.AreaName,
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        TypeName = Enum.GetName(typeof(AREA_TYPE), e.Type),
                        WarehouseId = e.WarehouseId,
                        WarehouseName = e.Warehouse.WhName,
                        AreaCode= e.AreaCode,
                        OUName = e.OU.OUName,
                        Status = Enum.GetName(typeof(AREA_STATUS), e.Status)
                    };
                    areaViewModels.Add(areaViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._reservoirAreaRepository.CountAsync(new ReservoirAreaSpecification(id,
                        null,ouId,wareHouseId,type, areaName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = areaViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = areaViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> AreaAssetChart(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                 
                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                    null,null,null,null,null,null,null,
                    null,null,null,null,ouId,null,null,null,
                    null,null);
                List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);
                List<string> lables = new List<string>();
                List<double> datas = new List<double>();
                var warehouseGroup = warehouseMaterials.GroupBy(w => w.ReservoirAreaId);
                foreach (var w in areas)
                {
                    lables.Add(w.AreaName);
                }
                foreach (var wg in warehouseGroup)
                {
                    double sumPrice = wg.Sum(w => w.Price);
                    datas.Add(sumPrice);
                }
               
                dynamic result = new ExpandoObject();
                result.labels = lables;
                result.datas = datas;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResultViewModel> AreaMaterialChart(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                 
                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                    null,null,null,null,null,null,null,
                    null,null,null,null,ouId,null,null,null,
                    null,null);
                List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);
                List<string> lables = new List<string>();
                List<double> datas = new List<double>();
                var warehouseGroup = warehouseMaterials.GroupBy(w => w.ReservoirAreaId);
                foreach (var w in areas)
                {
                    lables.Add(w.AreaName);
                }
                foreach (var wg in warehouseGroup)
                {
                    double sumCount = wg.Sum(w => w.MaterialCount);
                    datas.Add(sumCount);
                }
               
                dynamic result = new ExpandoObject();
                result.labels = lables;
                result.datas = datas;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResultViewModel> AreaWarehouseTrayChart(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                 
                WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null,null,
                    null,null,null,null,null,null,ouId,
                    null,null,null);
                
                List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                List<WarehouseTray> emptyWarehouseTrays = warehouseTrays.Where(w=>w.MaterialCount==0).ToList();
                List<WarehouseTray> notEmptyWarehouseTrays = warehouseTrays.Where(w=>w.MaterialCount>0).ToList();
                List<string> lables = new List<string>();
                List<double> datas1 = new List<double>();
                List<double> datas2 = new List<double>();
                var emptyWarehouseGroup = emptyWarehouseTrays.GroupBy(w => w.ReservoirAreaId);
                var notEmptyWarehouseGroup = notEmptyWarehouseTrays.GroupBy(w => w.ReservoirAreaId);
                foreach (var w in areas)
                {
                    lables.Add(w.AreaName);
                }
                foreach (var wg in emptyWarehouseGroup)
                {
                    double sumCount = wg.Count();
                    datas1.Add(sumCount);
                }
                foreach (var wg in notEmptyWarehouseGroup)
                {
                    double sumCount = wg.Count();
                    datas2.Add(sumCount);
                }
               
                dynamic result = new ExpandoObject();
                result.labels = lables;
                result.datas1 = datas1;
                result.datas2 = datas2;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResultViewModel> AreaEntryOutRecordChart(int ouId, int inOutType, int queryType)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                 
                InOutRecordSpecification inOutRecordSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    sCreateTime = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1).ToShortDateString() + " 00:00:00";
                    eCreateTime = DateTime.Now.ToString();
                }
                //本月
                else if (queryType == 2)
                {
                    sCreateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1).ToShortDateString() + " 00:00:00";
                    eCreateTime = DateTime.Now.ToString();
                }
                //本季度
                else if(queryType == 3)
                {
                    sCreateTime = DateTime.Now.AddMonths(0 - ((DateTime.Now.Month - 1) % 3)).ToShortDateString() + " 00:00:00";
                    eCreateTime = DateTime.Now.ToString();
                }
                //本年
                else if(queryType == 4)
                {
                    sCreateTime = DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1).ToShortDateString() + " 00:00:00";
                    eCreateTime = DateTime.Now.ToString();
                }
                
                inOutRecordSpec = new InOutRecordSpecification(null, null,
                    inOutType, ouId, null, null, null, null,null,null,
                    new List<int> {Convert.ToInt32(ORDER_STATUS.完成)}, null, null, sCreateTime, eCreateTime);
                
                List<InOutRecord> inOutRecords = await this._inOutRecordRepository.ListAsync(inOutRecordSpec);
                List<string> lables = new List<string>();
                List<double> datas = new List<double>();
                var inOutGroup = inOutRecords.GroupBy(w => w.ReservoirAreaId);
                foreach (var w in areas)
                {
                    lables.Add(w.AreaName);
                }
                foreach (var wg in inOutGroup)
                {
                    double sumCount = wg.Count();
                    datas.Add(sumCount);
                }
                dynamic result = new ExpandoObject();
                result.labels = lables;
                result.datas = datas;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }


        public async Task<ResponseResultViewModel> AssignLocation(LocationViewModel locationViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                if(!locationViewModel.ReservoirAreaId.HasValue)throw new Exception("分配货位,库区编号不能为空！");
                await this._reservoirAreaService.AssignLocation(locationViewModel.ReservoirAreaId.Value, locationViewModel.LocationIds);
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = string.Format("子库存[{0}],分配货位！",locationViewModel.ReservoirAreaId),
                    Founder = locationViewModel.Tag?.ToString(),
                    CreateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                    LogDesc = ex.StackTrace,
                    CreateTime = DateTime.Now
                });
            }
            return response;
        }
        
        
    }
}
