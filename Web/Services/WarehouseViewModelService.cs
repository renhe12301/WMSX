using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using System.Dynamic;
using System.Linq;
using ApplicationCore.Entities.FlowRecord;

namespace Web.Services
{
    public class WarehouseViewModelService : IWarehouseViewModelService
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IAsyncRepository<Warehouse> _wareHouseRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<InOutRecord> _inOutRecordRepository;
        public WarehouseViewModelService(IWarehouseService warehouseService,
                                         IAsyncRepository<Warehouse> wareHouseRepository,
                                         IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
                                         IAsyncRepository<WarehouseTray> warehouseTrayRepository,
                                         IAsyncRepository<InOutRecord> inOutRecordRepository)
        {
            this._warehouseService = warehouseService;
            this._wareHouseRepository = wareHouseRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._inOutRecordRepository = inOutRecordRepository;
        }

        public async Task<ResponseResultViewModel> GetWarehouses(int? pageIndex,
            int? itemsPage, int? id,int? ouId, string whName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Warehouse> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new WarehousePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,ouId,whName);
                }
                else
                {
                    baseSpecification = new WarehouseSpecification(id, ouId, whName,null);
                }
                var warehouses = await this._wareHouseRepository.ListAsync(baseSpecification);
                List<WarehouseViewModel> warehouseViewModels = new List<WarehouseViewModel>();
                warehouses.ForEach(e =>
                {
                    WarehouseViewModel wareHouseViewModel = new WarehouseViewModel
                    {
                        WhName = e.WhName,
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        OUName= e.OU.OUName,
                        Status = Enum.GetName(typeof(WAREHOUSE_STATUS), e.Status)
                    };
                    warehouseViewModels.Add(wareHouseViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._wareHouseRepository.CountAsync(new WarehouseSpecification(id, ouId,  whName,null));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = warehouseViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = warehouseViewModels;
                }
               
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> WarehouseAssetChart(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                    null,null,null,null,null,null,null,
                    null,null,null,null,ouId,null,null,null,
                    null,null);
                List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);
                List<string> lables = new List<string>();
                List<double> datas = new List<double>();
                var warehouseGroup = warehouseMaterials.GroupBy(w => w.WarehouseId);
                foreach (var w in warehouses)
                {
                    lables.Add(w.WhName);
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

        public async Task<ResponseResultViewModel> WarehouseMaterialChart(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                    null,null,null,null,null,null,null,
                    null,null,null,null,ouId,null,null,null,
                    null,null);
                List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);
                List<string> lables = new List<string>();
                List<double> datas = new List<double>();
                var warehouseGroup = warehouseMaterials.GroupBy(w => w.WarehouseId);
                foreach (var w in warehouses)
                {
                    lables.Add(w.WhName);
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

        public async Task<ResponseResultViewModel> WarehouseTrayChart(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null,null,
                    null,null,null,null,null,null,ouId,
                    null,null);
                
                List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                List<WarehouseTray> emptyWarehouseTrays = warehouseTrays.Where(w=>w.MaterialCount==0).ToList();
                List<WarehouseTray> notEmptyWarehouseTrays = warehouseTrays.Where(w=>w.MaterialCount>0).ToList();
                List<string> lables = new List<string>();
                List<double> datas1 = new List<double>();
                List<double> datas2 = new List<double>();
                var emptyWarehouseGroup = emptyWarehouseTrays.GroupBy(w => w.WarehouseId);
                var notEmptyWarehouseGroup = notEmptyWarehouseTrays.GroupBy(w => w.WarehouseId);
                foreach (var w in warehouses)
                {
                    lables.Add(w.WhName);
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

        public async Task<ResponseResultViewModel> WarehouseEntryOutRecordChart(int ouId,int inOutType, int queryType)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
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
                var inOutGroup = inOutRecords.GroupBy(w => w.WarehouseId);
                foreach (var w in warehouses)
                {
                    lables.Add(w.WhName);
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
    }
}
