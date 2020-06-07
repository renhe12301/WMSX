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

        public StatisticalViewModelService(IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<ReservoirArea> reservoirAreaRepository,
            IAsyncRepository<InOutTask> inOutTaskRepository,
            IAsyncRepository<Order> orderRepository,
            IAsyncRepository<SubOrder> subOrderRepository
            )
        {
            this._warehouseMaterialRepository = warehouseMaterialRepository;
            this._wareHouseRepository = warehouseRepository;
            this._reservoirAreaRepository = reservoirAreaRepository;
            this._inOutTaskRepository = inOutTaskRepository;
            this._orderRepository = orderRepository;
            this._subOrderRepository = subOrderRepository;
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
             
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    wdatas.Add(warehouseMaterials.Where(t=>t.WarehouseId==w.Id).Count());
                }
               
                dynamic result = new ExpandoObject();
                result.warehouseLabels = wlables;
                result.warehouseDatas = wdatas;
                
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                List<string> alables = new List<string>();
                List<double> adatas = new List<double>();
              
                foreach (var area in areas)
                {
                    alables.Add(area.AreaName);
                    adatas.Add(warehouseMaterials.Where(t=>t.ReservoirAreaId==area.Id).Count());
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
              
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    wdatas.Add(inOutRecords.Where(t=>t.WarehouseId==w.Id).Count());
                }
               
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                List<string> alables = new List<string>();
                List<double> adatas = new List<double>();
             
                foreach (var area in areas)
                {
                    alables.Add(area.AreaName);
                    adatas.Add(inOutRecords.Where(t=>t.ReservoirAreaId==area.Id).Count());
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
              
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    wdatas.Add(inOutRecords.Where(t=>t.WarehouseId==w.Id).Count());
                }
                ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(null,null,ouId,null,null,null);
                List<ReservoirArea> areas = await this._reservoirAreaRepository.ListAsync(reservoirAreaSpec);
                List<string> alables = new List<string>();
                List<double> adatas = new List<double>();
                
                foreach (var area in areas)
                {
                    alables.Add(area.AreaName);
                    adatas.Add(inOutRecords.Where(t=>t.ReservoirAreaId==area.Id).Count());
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
                    Convert.ToInt32(ORDER_TYPE.入库退库),
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
                    sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库退库)).Count();
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

                orderSpec = new SubOrderSpecification(null,null,null,orderTypeIds,null,
                    null,null,ouId,null,null,null,null,null,
                    null,null,null,null,null);
                
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
                    Convert.ToInt32(ORDER_TYPE.入库退库)
                };

                orderSpec = new SubOrderSpecification(null,null,null,orderTypeIds,null,
                    null,null,ouId,null,null,null,null,null,
                    null,null,null,null,null);
                
                List<SubOrder> orders = await this._subOrderRepository.ListAsync(orderSpec);
                List<string> wlables = new List<string>();
                List<double> wdatas = new List<double>();
                List<double> wdatas2 = new List<double>();
                foreach (var w in warehouses)
                {
                    wlables.Add(w.WhName);
                    int sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.出库领料)).Count();
                    wdatas.Add(sumCount);
                    sumCount = orders.Where(o=>o.OrderTypeId==Convert.ToInt32(ORDER_TYPE.入库退库)).Count();
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

       
    }
}