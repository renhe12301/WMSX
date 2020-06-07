using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services
{
    public class StatisticalViewModelService:IStatisticalViewModelService
    {
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<Warehouse> _wareHouseRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;

        public StatisticalViewModelService(IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<ReservoirArea> reservoirAreaRepository,
            IAsyncRepository<InOutTask> inOutTaskRepository)
        {
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._wareHouseRepository = warehouseRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._inOutTaskRepository = inOutTaskRepository;
        }

        public async Task<ResponseResultViewModel> MaterialChart(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                    null,null,null,null,null,null,null,
                    null,null,null,null,ouId,null,null,null,
                    null,null,null,null);
                List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);
                List<string> wlables = new List<string>();
                List<double> wdatas = new List<double>();
                var warehouseGroup = warehouseMaterials.GroupBy(w => w.WarehouseId);
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                }
                foreach (var wg in warehouseGroup)
                {
                    double sumCount = wg.Sum(w => w.MaterialCount);
                    wdatas.Add(sumCount);
                }
               
                dynamic result = new ExpandoObject();
                result.warehouseLabels = wlables;
                result.warehouseDatas = wdatas;
                
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                List<string> alables = new List<string>();
                List<double> adatas = new List<double>();
                var areaGroup = warehouseMaterials.GroupBy(w => w.ReservoirAreaId);
                foreach (var area in areas)
                {
                    alables.Add(area.AreaName);
                }
                foreach (var area in areaGroup)
                {
                    double sumCount = area.Sum(w => w.MaterialCount);
                    adatas.Add(sumCount);
                }
                result.areaLabels = alables;
                result.areaDatas = adatas;
                
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResultViewModel> InRecordChart(int ouId, int queryType)
        {
             ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                InOutTaskSpecification inOutRecordSpec = null;
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
                
                inOutRecordSpec = new InOutTaskSpecification(null, null,
                    null,null, null, new List<int> {Convert.ToInt32(ORDER_STATUS.完成)}, null,
                    new List<int>{Convert.ToInt32(TASK_TYPE.物料入库)}, null,ouId,null,
                    null, null, sCreateTime, eCreateTime , null,null);
                
                List<InOutTask> inOutRecords = await this._inOutTaskRepository.ListAsync(inOutRecordSpec);
                
                List<string> wlables = new List<string>();
                List<double> wdatas = new List<double>();
                var wGroup = inOutRecords.GroupBy(w => w.WarehouseId);
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                }
                foreach (var wg in wGroup)
                {
                    double sumCount = wg.Count();
                    wdatas.Add(sumCount);
                }
                
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                List<string> alables = new List<string>();
                List<double> adatas = new List<double>();
                var aGroup = inOutRecords.GroupBy(w => w.ReservoirAreaId);
                foreach (var area in areas)
                {
                    alables.Add(area.AreaName);
                }
                foreach (var ag in aGroup)
                {
                    double sumCount = ag.Count();
                    adatas.Add(sumCount);
                }
                dynamic result = new ExpandoObject();
                result.warehouselabels = wlables;
                result.warehousedatas = wdatas;
                result.arealables = wlables;
                result.areadatas = wdatas;

                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public Task<ResponseResultViewModel> OutRecordChart(int ouId, int queryType)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseResultViewModel> InSubOrderChart(int ouId, int queryType)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseResultViewModel> OutSubOrderChart(int ouId, int queryType)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseResultViewModel> InOrderChart(int ouId, int queryType)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseResultViewModel> OutOrderChart(int ouId, int queryType)
        {
            throw new System.NotImplementedException();
        }
    }
}