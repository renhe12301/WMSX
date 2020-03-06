using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.BasicInformation;

using ApplicationCore.Specifications;
using ApplicationCore.Misc;
using System.Linq;
using ApplicationCore.Entities.StockManager;
using LogRecord = ApplicationCore.Entities.FlowRecord.LogRecord;

namespace Web.Services
{
    public class OrderViewModelService:IOrderViewModelService
    {

        private readonly IOrderService _orderService;
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<ReservoirArea> _areaRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly ILogRecordService _logRecordService;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;

        public OrderViewModelService(IOrderService orderService,
                                       IAsyncRepository<Order> orderRepository,
                                       IAsyncRepository<ReservoirArea> areaRepository,
                                       IAsyncRepository<Warehouse> warehouseRepository,
                                       ILogRecordService logRecordService,
                                       IAsyncRepository<OrderRow> orderRowRepository,
                                       IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository)
        {
            this._orderService = orderService;
            this._orderRepository = orderRepository;
            this._areaRepository = areaRepository;
            this._warehouseRepository = warehouseRepository;
            this._logRecordService = logRecordService;
            this._orderRowRepository = orderRowRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
        }
        
        public async Task<ResponseResultViewModel> GetOrders(int? pageIndex,int? itemsPage,
            int?id,string orderNumber, int? orderTypeId,string status, string applyUserCode, string approveUserCode,
            int? employeeId,string employeeName,string sApplyTime, string eApplyTime, string sApproveTime,
            string eApproveTime,string sCreateTime,string eCreateTime,string sFinishTime,string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Order> spec = null;
                List<int> orderStatuss = null;
                if (!string.IsNullOrEmpty(status))
                {
                    orderStatuss = status.Split(new char[]{','}, 
                                                  StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    spec = new OrderPaginatedSpecification(pageIndex.Value,itemsPage.Value,id, orderNumber, orderTypeId,
                        orderStatuss, applyUserCode, approveUserCode,employeeId,employeeName, sApplyTime, eApplyTime, sApproveTime, eApproveTime,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }
                else
                {
                    spec = new OrderSpecification(id, orderNumber, orderTypeId,
                        orderStatuss, applyUserCode, approveUserCode, employeeId,employeeName,sApplyTime, eApplyTime, 
                        sApproveTime, eApproveTime,sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }
                var orders = await this._orderRepository.ListAsync(spec);
                List<OrderViewModel> orderViewModels = new List<OrderViewModel>();

                orders.ForEach(e =>
                {
                    OrderViewModel orderViewModel = new OrderViewModel
                    {
                        Id = e.Id,
                        OrderNumber = e.OrderNumber,
                        OrderType = e.OrderType?.TypeName,
                        OrderTypeId = e.OrderTypeId,
                        ApplyUserCode = e.ApplyUserCode,
                        ApproveUserCode = e.ApproveUserCode,
                        ApplyTime = e.ApplyTime?.ToString(),
                        ApproveTime = e.ApproveTime?.ToString(),
                        CallingParty = e.CallingParty,
                        Progress = e.Progress,
                        Memo = e.Memo,
                        CreateTime = e.CreateTime?.ToString(),
                        FinishTime = e.FinishTime?.ToString(),
                        WarehouseId = e.WarehouseId,
                        WarehouseName = e.Warehouse?.WhName,
                        OUName = e.OU?.OUName,
                        OUId = e.OUId,
                        EBSProjectId = e.EBSProjectId.GetValueOrDefault(),
                        ProjectName = e.EBSProject?.ProjectName,
                        SupplierId = e.SupplierId.GetValueOrDefault(),
                        SupplierName = e.Supplier?.SupplierName,
                        SupplierSiteId = e.SupplierSiteId.GetValueOrDefault(),
                        SupplierSiteName = e.SupplierSite?.SiteName,
                        Currency = e.Currency,
                        TotalAmount = e.TotalAmount,
                        Status = e.Status,
                        StatusStr = Enum.GetName(typeof(ORDER_STATUS), e.Status),
                        EmployeeId = e.EmployeeId,
                        EmployeeName = e.Employee?.UserName
                    };
                    orderViewModels.Add(orderViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._orderRepository.CountAsync(new OrderSpecification(id, orderNumber, orderTypeId,
                        orderStatuss, applyUserCode, approveUserCode, employeeId,employeeName,sApplyTime, eApplyTime, 
                        sApproveTime, eApproveTime,sCreateTime, eCreateTime, sFinishTime, eFinishTime));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = orderViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = orderViewModels;
                }
              
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetTKOrderMaterials(int ouId, int warehouseId, int? areaId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                OrderRowSpecification orderRowSpec = new OrderRowSpecification(null,null,null,
                     new List<int>{Convert.ToInt32(ORDER_STATUS.待处理),Convert.ToInt32(ORDER_STATUS.执行中)},null,
                     null,null,null);
                 List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                 List<OrderRow> tkOrderRows = orderRows.Where(or => or.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库)).ToList();
                 ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(areaId,null,ouId,warehouseId,null,null);
                 List<ReservoirArea> areas = await this._areaRepository.ListAsync(reservoirAreaSpec);
                 
                 WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,null,
                     null,null,null,null,null,null,null,
                     null,new List<int>(){Convert.ToInt32(TRAY_STEP.入库完成),Convert.ToInt32(TRAY_STEP.初始化)},
                     null,ouId,warehouseId,null,null,null);
                 List<WarehouseMaterial> allWarehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);

                 List<dynamic> result = new List<dynamic>();
                 foreach (var area in areas)
                 {
                     List<WarehouseMaterial> areaWarehouseMaterials =
                         allWarehouseMaterials.Where(a => a.ReservoirAreaId == area.Id).ToList();
                     var groupMaterial = areaWarehouseMaterials.GroupBy(g => g.MaterialDicId);
                     foreach (var mg in groupMaterial)
                     {
                         dynamic dyn = new ExpandoObject();
                         dyn.MaterialId = mg.First().MaterialDic.Id;
                         dyn.MaterialName = mg.First().MaterialDic.MaterialName;
                         int materialCount = mg.Sum(m => m.MaterialCount);
                         double totalAmount = mg.Sum(m => m.Amount);
                         dyn.MaterialCount = materialCount;
                         int occCount = tkOrderRows.Where(or => or.MaterialDicId == mg.First().MaterialDicId)
                             .Sum(or => or.PreCount);
                         dyn.RemainingCount = materialCount - occCount;
                         dyn.OccCount = occCount;
                         dyn.MaterialCode = mg.First().MaterialDic.MaterialCode;
                         dyn.MaterialSpec = mg.First().MaterialDic.Spec;
                         dyn.TKCount = 0;
                         dyn.AreaId = mg.First().ReservoirAreaId;
                         dyn.AreaName = mg.First().ReservoirArea?.AreaName;
                         dyn.Price = mg.First().Price;
                         dyn.Amount = totalAmount;
                         dyn.OUId = mg.First().OUId;
                         dyn.WarehouseId = mg.First().WarehouseId;
                         result.Add(dyn);
                     }
                 }

                 response.Data = result;

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

        public async Task<ResponseResultViewModel> CreateOutOrder(OrderViewModel orderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                DateTime now = DateTime.Now;
                Order order = new Order
                {
                    Id = orderViewModel.Id.GetValueOrDefault(),
                    OUId = orderViewModel.OUId,
                    WarehouseId = orderViewModel.WarehouseId,
                    OrderNumber = orderViewModel.OrderNumber??"TK_Order_"+now.Ticks,
                    CreateTime = now,
                    ApplyTime = now,
                    ApplyUserCode = orderViewModel.ApplyUserCode,
                    ApproveTime = now,
                    ApproveUserCode = orderViewModel.ApproveUserCode,
                    CallingParty = orderViewModel.CallingParty,
                    OrderTypeId = orderViewModel.OrderTypeId,
                    Status = 0,
                    EmployeeId = orderViewModel.EmployeeId,
                    Tag = orderViewModel.Tag
                    
                };
                List<OrderRow> orderRows = new List<OrderRow>();
              
                orderViewModel.OrderRows.ForEach(async(or) =>
                {
                    OrderRow orderRow = new OrderRow
                    {
                        Id = or.Id.GetValueOrDefault(),
                        RowNumber = or.RowNumber??"TK_Order_"+now.Ticks,
                        CreateTime=now,
                        PreCount=or.PreCount,
                        ReservoirAreaId = or.ReservoirAreaId,
                        MaterialDicId = or.MaterialDicId,
                        Price = or.Price
                    };
                    orderRows.Add(orderRow);
                });
                order.OrderRow = orderRows;
                await this._orderService.CreateOutOrder(order);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> SortingOrder(OrderRowViewModel orderRow)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._orderService.SortingOrder(orderRow.OrderId,
                    orderRow.Id.GetValueOrDefault(), orderRow.Sorting, orderRow.BadCount,
                    orderRow.TrayCode, orderRow.ReservoirAreaId.GetValueOrDefault(),
                    orderRow.Tag);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> OrderOut(OrderRowBatchViewModel orderRowBatchViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._orderService.OrderOut(orderRowBatchViewModel.OrderId.GetValueOrDefault(),
                    orderRowBatchViewModel.OrderRowId.GetValueOrDefault(),
                    orderRowBatchViewModel.ReservoirAreaId,orderRowBatchViewModel.BatchCount,orderRowBatchViewModel.Type,
                    orderRowBatchViewModel.Tag);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> CloseOrder(OrderViewModel orderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._orderService.CloseOrder(orderViewModel.Id.GetValueOrDefault(),orderViewModel.Tag);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> CloseOrderRow(OrderRowViewModel orderRowViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._orderService.CloseOrderRow(orderRowViewModel.Id.GetValueOrDefault(),orderRowViewModel.Tag);
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
