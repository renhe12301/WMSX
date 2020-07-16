using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Services
{
    public class StatisticalViewModelService:IStatisticalViewModelService
    {
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        private readonly IAsyncRepository<Warehouse> _wareHouseRepository;
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;
        private readonly IAsyncRepository<Location> _locationRepository;

        public StatisticalViewModelService(IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<ReservoirArea> reservoirAreaRepository,
            IAsyncRepository<InOutTask> inOutTaskRepository,
            IAsyncRepository<Order> orderRepository,
            IAsyncRepository<SubOrder> subOrderRepository,
            IAsyncRepository<WarehouseTray> warehouseTrayRepository,
            IAsyncRepository<Location> locationRepository
            )
        {
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._wareHouseRepository = warehouseRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._inOutTaskRepository = inOutTaskRepository;
            this._orderRepository = orderRepository;
            this._subOrderRepository = subOrderRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;
            this._locationRepository = locationRepository;
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
                List<string> alables = new List<string>();
                List<double> adatas = new List<double>();
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    wdatas.Add(warehouseMaterials.Where(t=>t.WarehouseId==w.Id).Sum(m=>m.MaterialCount));
                    
                    ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,null,w.Id,null,null);
                    List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);

                    foreach (var area in areas)
                    {
                        alables.Add(area.AreaName);
                        adatas.Add(warehouseMaterials.Where(t=>t.ReservoirAreaId==area.Id).Sum(m=>m.MaterialCount));
                    }
                }
               
                dynamic result = new ExpandoObject();
                result.warehouseLabels = wlables;
                result.warehouseDatas = wdatas;
                
              
               
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
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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
                List<string> alables = new List<string>();
                List<double> adatas = new List<double>();
              
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    wdatas.Add(inOutRecords.Where(t=>t.WarehouseId==w.Id).Count());
                    
                    ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,null,w.Id,null,null);
                    List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
              
             
                    foreach (var area in areas)
                    {
                        alables.Add(area.AreaName);
                        adatas.Add(inOutRecords.Where(t=>t.ReservoirAreaId==area.Id).Count());
                    }

                }
               
               
                dynamic result = new ExpandoObject();
                result.warehouseLabels = wlables;
                result.warehouseDatas = wdatas;
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
        

        public async Task<ResponseResultViewModel> OutRecordChart(int ouId, int queryType)
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
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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
                    new List<int>{Convert.ToInt32(TASK_TYPE.物料出库)}, null,ouId,null,
                    null, null, sCreateTime, eCreateTime , null,null);
                
                List<InOutTask> inOutRecords = await this._inOutTaskRepository.ListAsync(inOutRecordSpec);
                
                List<string> wlables = new List<string>();
                List<double> wdatas = new List<double>();
                List<string> alables = new List<string>();
                List<double> adatas = new List<double>();
              
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    wdatas.Add(inOutRecords.Where(t=>t.WarehouseId==w.Id).Count());
                    
                    ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,null,w.Id,null,null);
                    List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                
                    foreach (var area in areas)
                    {
                        alables.Add(area.AreaName);
                        adatas.Add(inOutRecords.Where(t=>t.ReservoirAreaId==area.Id).Count());
                    }
                }

                dynamic result = new ExpandoObject();
                result.warehouselabels = wlables;
                result.warehousedatas = wdatas;
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

        
        public async Task<ResponseResultViewModel> InOrderChart(int ouId, int queryType)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                OrderSpecification orderSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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

                List<int> orderTypeIds = new List<int>
                {
                    Convert.ToInt32(ORDER_TYPE.入库接收),
                    Convert.ToInt32(ORDER_TYPE.入库退料)
                };

                orderSpec = new OrderSpecification(null,null,null,orderTypeIds,null, ouId
                ,null,null,null,null,null,null,null,
                null,null,null,null,null,sCreateTime,eCreateTime,
                null,null);
                
                List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
                List<string> wlables = new List<string>();
                List<double> wdatas = new List<double>();
                List<double> wdatas2 = new List<double>();
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    int sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库接收)).Count();
                    wdatas.Add(sumCount);
                    sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库退料)).Count();
                    wdatas2.Add(sumCount);
                }
               
                dynamic result = new ExpandoObject();
                result.warehouseLabels = wlables;
                result.warehouseDatas = wdatas;
                result.warehouseDatas2 = wdatas2;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        
        public async Task<ResponseResultViewModel> OutOrderChart(int ouId, int queryType)
        {
             ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                OrderSpecification orderSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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

                List<int> orderTypeIds = new List<int>
                {
                    Convert.ToInt32(ORDER_TYPE.出库领料),
                    Convert.ToInt32(ORDER_TYPE.出库退库),
                };

                orderSpec = new OrderSpecification(null,null,null,orderTypeIds,null, ouId
                ,null,null,null,null,null,null,null,
                null,null,null,null,null,sCreateTime,eCreateTime,
                null,null);
                
                List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
                List<string> wlables = new List<string>();
                List<double> wdatas = new List<double>();
                List<double> wdatas2 = new List<double>();
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    int sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库领料)).Count();
                    wdatas.Add(sumCount);
                    sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库退库)).Count();
                    wdatas2.Add(sumCount);
                }
               
                dynamic result = new ExpandoObject();
                result.warehouseLabels = wlables;
                result.warehouseDatas = wdatas;
                result.warehouseDatas2 = wdatas2;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
        

        public async Task<ResponseResultViewModel> InSubOrderChart(int ouId, int queryType)
        {
           ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                SubOrderSpecification orderSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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

                List<int> orderTypeIds = new List<int>
                {
                    Convert.ToInt32(ORDER_TYPE.入库接收),
                    Convert.ToInt32(ORDER_TYPE.入库退料)
                };

                orderSpec = new SubOrderSpecification(null,null,null,orderTypeIds,null,null,
                    null,null,null,ouId,null,null,null,null,null,
                    null,sCreateTime,eCreateTime,null,null);
                
                List<SubOrder> orders = await this._subOrderRepository.ListAsync(orderSpec);
                List<string> wlables = new List<string>();
                List<double> wdatas = new List<double>();
                List<double> wdatas2 = new List<double>();
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    int sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库接收)).Count();
                    wdatas.Add(sumCount);
                    sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库退料)).Count();
                    wdatas2.Add(sumCount);
                }
               
                dynamic result = new ExpandoObject();
                result.warehouseLabels = wlables;
                result.warehouseDatas = wdatas;
                result.warehouseDatas2 = wdatas2;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
       

        public async Task<ResponseResultViewModel> OutSubOrderChart(int ouId, int queryType)
        {
           ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                 
                SubOrderSpecification orderSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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

                List<int> orderTypeIds = new List<int>
                {
                    Convert.ToInt32(ORDER_TYPE.出库领料),
                    Convert.ToInt32(ORDER_TYPE.出库退库)
                };

                orderSpec = new SubOrderSpecification(null,null,null,orderTypeIds,null,null,
                    null,null,null,ouId,null,null,null,null,null,
                    null,sCreateTime,eCreateTime,null,null);
                
                List<SubOrder> orders = await this._subOrderRepository.ListAsync(orderSpec);
                List<string> wlables = new List<string>();
                List<double> wdatas = new List<double>();
                List<double> wdatas2 = new List<double>();
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    int sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库领料)).Count();
                    wdatas.Add(sumCount);
                    sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库退库)).Count();
                    wdatas2.Add(sumCount);
                }
               
                dynamic result = new ExpandoObject();
                result.warehouseLabels = wlables;
                result.warehouseDatas = wdatas;
                result.warehouseDatas2 = wdatas2;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
        
        
        public async Task<ResponseResultViewModel> MaterialSheet(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                
                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                    null,null,null,null,null,null,null,
                    null,null,null,null,ouId,null,null,null,
                    null,null,null,null);
                List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);
                
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                List<ReservoirArea> areas = new List<ReservoirArea>();
                foreach (var warehouse in warehouses)
                {
                    ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,null,warehouse.Id,null,null);
                    List<ReservoirArea> wareas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                    areas.AddRange(wareas);
                }
                
                List<ReservoirAreaViewModel> reservoirAreaViewModels = new List<ReservoirAreaViewModel> ();
                var groupAreas = areas.GroupBy(g => g.WarehouseId);
                foreach (var groupArea in groupAreas)
                {
                    var totalMaterilCnt = warehouseMaterials.Where(m => m.WarehouseId == groupArea.Key)
                        .Sum(m => m.MaterialCount);
                    
                    foreach (var gas in groupArea)
                    {
                        var materilCnt = warehouseMaterials.Where(m => m.ReservoirAreaId == gas.Id)
                            .Sum(m => m.MaterialCount);
                        ReservoirAreaViewModel areaViewModel = new ReservoirAreaViewModel
                        {
                            AreaName =gas.AreaName,
                            Id = gas.Id,
                            AreaCode= gas.AreaCode,
                            WarehouseName = gas.Warehouse?.WhName,
                            StatisticalCount = materilCnt.ToString(),
                            TotalStatisticalCount = totalMaterilCnt.ToString()
                        };
                        reservoirAreaViewModels.Add(areaViewModel);
                    }
                }

                response.Data = reservoirAreaViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
        
        
         public async Task<ResponseResultViewModel> InRecordSheet(int ouId, int queryType)
        {
           ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                InOutTaskSpecification inOutRecordSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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
                
               
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                List<ReservoirArea> areas = new List<ReservoirArea>();
                foreach (var warehouse in warehouses)
                {
                    ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,null,warehouse.Id,null,null);
                    List<ReservoirArea> wareas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                    areas.AddRange(wareas);
                }

                List<ReservoirAreaViewModel> reservoirAreaViewModels = new List<ReservoirAreaViewModel> ();
                var groupAreas = areas.GroupBy(g => g.WarehouseId);
                foreach (var groupArea in groupAreas)
                {
                    var totalCnt = inOutRecords.Where(m => m.WarehouseId == groupArea.Key).Count();
                    
                    foreach (var gas in groupArea)
                    {
                        var cnt = inOutRecords.Where(m => m.ReservoirAreaId == gas.Id)
                            .Count();
                        ReservoirAreaViewModel areaViewModel = new ReservoirAreaViewModel
                        {
                            AreaName =gas.AreaName,
                            Id = gas.Id,
                            AreaCode= gas.AreaCode,
                            WarehouseName = gas.Warehouse?.WhName,
                            StatisticalCount = cnt.ToString(),
                            TotalStatisticalCount = totalCnt.ToString()
                        };
                        reservoirAreaViewModels.Add(areaViewModel);
                    }
                }

                response.Data = reservoirAreaViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
        
        public async Task<ResponseResultViewModel> OutRecordSheet(int ouId, int queryType)
        {
           ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                InOutTaskSpecification inOutRecordSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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
                    new List<int>{Convert.ToInt32(TASK_TYPE.物料出库)}, null,ouId,null,
                    null, null, sCreateTime, eCreateTime , null,null);
                
                List<InOutTask> inOutRecords = await this._inOutTaskRepository.ListAsync(inOutRecordSpec);
                
                WarehouseSpecification warehouseSpec = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpec);
                List<ReservoirArea> areas = new List<ReservoirArea>();
                foreach (var warehouse in warehouses)
                {
                    ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,null,warehouse.Id,null,null);
                    List<ReservoirArea> wareas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                    areas.AddRange(wareas);
                }
                
                List<ReservoirAreaViewModel> reservoirAreaViewModels = new List<ReservoirAreaViewModel> ();
                var groupAreas = areas.GroupBy(g => g.WarehouseId);
                foreach (var groupArea in groupAreas)
                {
                    var totalCnt = inOutRecords.Where(m => m.WarehouseId == groupArea.Key).Count();
                    
                    foreach (var gas in groupArea)
                    {
                        var cnt = inOutRecords.Where(m => m.ReservoirAreaId == gas.Id)
                            .Count();
                        ReservoirAreaViewModel areaViewModel = new ReservoirAreaViewModel
                        {
                            AreaName =gas.AreaName,
                            Id = gas.Id,
                            AreaCode= gas.AreaCode,
                            WarehouseName = gas.Warehouse?.WhName,
                            StatisticalCount = cnt.ToString(),
                            TotalStatisticalCount = totalCnt.ToString()
                        };
                        reservoirAreaViewModels.Add(areaViewModel);
                    }
                }

                response.Data = reservoirAreaViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
        
        public async Task<ResponseResultViewModel> OutSubOrderSheet(int ouId, int queryType)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                SubOrderSpecification orderSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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

                List<int> orderTypeIds = new List<int>
                {
                    Convert.ToInt32(ORDER_TYPE.出库领料),
                    Convert.ToInt32(ORDER_TYPE.出库退库)
                };

                orderSpec = new SubOrderSpecification(null,null,null,orderTypeIds,null,null,
                    null,null,null,ouId,null,null,null,null,null,
                    null,sCreateTime,eCreateTime,null,null);
                
                List<SubOrder> orders = await this._subOrderRepository.ListAsync(orderSpec);
                
                WarehouseSpecification warehouseSpecification = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpecification);
                List<dynamic> result = new List<dynamic>();
                foreach (var warehouse in warehouses)
                {
                    var totalCnt = orders.Where(m => m.WarehouseId == warehouse.Id).Count();
                    dynamic dyn = new ExpandoObject();
                    dyn.WarehouseName = warehouse.WhName;
                    dyn.OrderType = "领料";
                    dyn.TotalStatisticalCount = totalCnt;
                    dyn.StatisticalCount = orders.Where(m => m.WarehouseId == warehouse.Id&&m.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库领料)).Count();
                    
                    dynamic dyn2 = new ExpandoObject();
                    dyn2.WarehouseName = warehouse.WhName;
                    dyn2.OrderType = "退库";
                    dyn2.TotalStatisticalCount = totalCnt;
                    dyn2.StatisticalCount = orders.Where(m => m.WarehouseId == warehouse.Id&&m.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库退库)).Count();
                    
                    result.Add(dyn);
                    result.Add(dyn2);
                }

                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
        
        public async Task<ResponseResultViewModel> InOrderSheet(int ouId, int queryType)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                OrderSpecification orderSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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

                List<int> orderTypeIds = new List<int>
                {
                    Convert.ToInt32(ORDER_TYPE.入库接收),
                    Convert.ToInt32(ORDER_TYPE.入库退料)
                };

                orderSpec = new OrderSpecification(null,null,null,orderTypeIds,null, ouId
                    ,null,null,null,null,null,null,null,
                    null,null,null,null,null,sCreateTime,eCreateTime,
                    null,null);
                
                List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
                
                WarehouseSpecification warehouseSpecification = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpecification);
                List<dynamic> result = new List<dynamic>();
                foreach (var warehouse in warehouses)
                {
                    var totalCnt = orders.Where(m => m.WarehouseId == warehouse.Id).Count();
                    dynamic dyn = new ExpandoObject();
                    dyn.WarehouseName = warehouse.WhName;
                    dyn.OrderType = "入库";
                    dyn.TotalStatisticalCount = totalCnt;
                    dyn.StatisticalCount = orders.Where(m => m.WarehouseId == warehouse.Id&&m.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库接收)).Count();
                    
                    dynamic dyn2 = new ExpandoObject();
                    dyn2.WarehouseName = warehouse.WhName;
                    dyn2.OrderType = "退料";
                    dyn2.TotalStatisticalCount = totalCnt;
                    dyn2.StatisticalCount = orders.Where(m => m.WarehouseId == warehouse.Id&&m.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库退料)).Count();
                    
                    result.Add(dyn);
                    result.Add(dyn2);
                }

                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }
        public async Task<ResponseResultViewModel> OutOrderSheet(int ouId, int queryType)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                OrderSpecification orderSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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

                List<int> orderTypeIds = new List<int>
                {
                    Convert.ToInt32(ORDER_TYPE.出库领料),
                    Convert.ToInt32(ORDER_TYPE.出库退库)
                };

                orderSpec = new OrderSpecification(null,null,null,orderTypeIds,null, ouId
                    ,null,null,null,null,null,null,null,
                    null,null,null,null,null,sCreateTime,eCreateTime,
                    null,null);
                
                List<Order> orders = await this._orderRepository.ListAsync(orderSpec);
                
                WarehouseSpecification warehouseSpecification = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpecification);
                List<dynamic> result = new List<dynamic>();
                foreach (var warehouse in warehouses)
                {
                    var totalCnt = orders.Where(m => m.WarehouseId == warehouse.Id).Count();
                    dynamic dyn = new ExpandoObject();
                    dyn.WarehouseName = warehouse.WhName;
                    dyn.OrderType = "领料";
                    dyn.TotalStatisticalCount = totalCnt;
                    dyn.StatisticalCount = orders.Where(m => m.WarehouseId == warehouse.Id&&m.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库领料)).Count();
                    
                    dynamic dyn2 = new ExpandoObject();
                    dyn2.WarehouseName = warehouse.WhName;
                    dyn2.OrderType = "退库";
                    dyn2.TotalStatisticalCount = totalCnt;
                    dyn2.StatisticalCount = orders.Where(m => m.WarehouseId == warehouse.Id&&m.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库退库)).Count();
                    
                    result.Add(dyn);
                    result.Add(dyn2);
                }

                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResultViewModel> InSubOrderSheet(int ouId, int queryType)
        {
          ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                SubOrderSpecification orderSpec = null;
                string sCreateTime = DateTime.Now.ToShortDateString()+" 00:00:00";
                string eCreateTime = DateTime.Now.ToString();
                //本周
                if (queryType == 1)
                {
                    int dayWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    dayWeek = dayWeek == 0 ? 7 : dayWeek;
                    sCreateTime = DateTime.Now.AddDays(-dayWeek).AddDays(1).ToShortDateString() + " 00:00:00";
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

                List<int> orderTypeIds = new List<int>
                {
                    Convert.ToInt32(ORDER_TYPE.入库接收),
                    Convert.ToInt32(ORDER_TYPE.入库退料)
                };

                orderSpec = new SubOrderSpecification(null,null,null,orderTypeIds,null,null,
                    null,null,null,ouId,null,null,null,null,null,
                    null,sCreateTime,eCreateTime,null,null);
                
                List<SubOrder> orders = await this._subOrderRepository.ListAsync(orderSpec);
                
                WarehouseSpecification warehouseSpecification = new WarehouseSpecification(null,ouId,null,null);
                List<Warehouse> warehouses = await this._wareHouseRepository.ListAsync(warehouseSpecification);
                List<dynamic> result = new List<dynamic>();
                foreach (var warehouse in warehouses)
                {
                    var totalCnt = orders.Where(m => m.WarehouseId == warehouse.Id).Count();
                    dynamic dyn = new ExpandoObject();
                    dyn.WarehouseName = warehouse.WhName;
                    dyn.OrderType = "入库";
                    dyn.TotalStatisticalCount = totalCnt;
                    dyn.StatisticalCount = orders.Where(m => m.WarehouseId == warehouse.Id&&m.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库接收)).Count();
                    
                    dynamic dyn2 = new ExpandoObject();
                    dyn2.WarehouseName = warehouse.WhName;
                    dyn2.OrderType = "退料";
                    dyn2.TotalStatisticalCount = totalCnt;
                    dyn2.StatisticalCount = orders.Where(m => m.WarehouseId == warehouse.Id&&m.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库退料)).Count();
                    
                    result.Add(dyn);
                    result.Add(dyn2);
                }

                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        
        public async Task<ResponseResultViewModel> PyWarehouseChart(int pyId)
        {
             ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                dynamic result = new ExpandoObject();
                LocationSpecification locationSpec = new LocationSpecification(null,null,null,null,pyId,null,null,
                    null,null,null,null,null,null,null);

                List<Location> locations = await  this._locationRepository.ListAsync(locationSpec);
                
                WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null,null,null,
                    null,null,null,null,null,null,null,null, pyId);
                List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                var norLocCnt = locations.Where(t => t.Status == Convert.ToInt32(LOCATION_STATUS.正常)&&t.Type==Convert.ToInt32(LOCATION_TYPE.仓库区货位))
                    .Count();
                var disLocCnt = locations.Where(t => t.Status == Convert.ToInt32(LOCATION_STATUS.禁用)&&t.Type==Convert.ToInt32(LOCATION_TYPE.仓库区货位))
                    .Count();
                var taskLocCnt = locations.Where(t => t.Status == Convert.ToInt32(LOCATION_STATUS.锁定)&&t.Type==Convert.ToInt32(LOCATION_TYPE.仓库区货位))
                    .Count();
                var emptyTrayLocCnt = locations.Where(t => t.InStock == 2&&t.Type==Convert.ToInt32(LOCATION_TYPE.仓库区货位)).Count();
                
                var emptyLocCnt = locations.Where(t => t.InStock == 0&&t.Type==Convert.ToInt32(LOCATION_TYPE.仓库区货位)).Count();
                
                var materialLocCnt = locations.Where(t => t.InStock==1&&t.Type==Convert.ToInt32(LOCATION_TYPE.仓库区货位)).Count();
                
                // var ouCnt = warehouseTrays.GroupBy(t=>t.OUId).Count();
                //
                // var warehouseCnt = warehouseTrays.GroupBy(t=>t.WarehouseId).Count();
                //
                // var areaCnt = warehouseTrays.GroupBy(t=>t.WarehouseId).Count();
                //
                // var supplierCnt = warehouseTrays.GroupBy(t=>t.SubOrder.SupplierId).Count();
                //
                // var supplierSiteCnt = warehouseTrays.GroupBy(t=>t.SubOrder.SupplierSiteId).Count();

                result.norLocCnt = norLocCnt;
                result.disLocCnt = disLocCnt;
                result.taskLocCnt = taskLocCnt;
                result.emptyLocCnt = emptyLocCnt;
                result.emptyTrayLocCnt = emptyTrayLocCnt;
                result.materialLocCnt = materialLocCnt;
                // result.ouCnt = ouCnt;
                // result.warehouseCnt = warehouseCnt;
                // result.areaCnt = areaCnt;
                // result.supplierCnt = supplierCnt;
                // result.supplierSiteCnt = supplierSiteCnt;
                
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