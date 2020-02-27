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

        public OrderViewModelService(IOrderService orderService,
                                       IAsyncRepository<Order> orderRepository,
                                       IAsyncRepository<ReservoirArea> areaRepository,
                                       IAsyncRepository<Warehouse> warehouseRepository,
                                       ILogRecordService _logRecordService)
        {
            this._orderService = orderService;
            this._orderRepository = orderRepository;
            this._areaRepository = areaRepository;
            this._warehouseRepository = warehouseRepository;
            this._logRecordService = _logRecordService;
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
                        EBSProjectId = e.EBSProjectId,
                        ProjectName = e.EBSProject?.ProjectName,
                        SupplierId = e.SupplierId,
                        SupplierName = e.Supplier?.SupplierName,
                        SupplierSiteId = e.SupplierSiteId,
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

        public async Task<ResponseResultViewModel> CreateOrder(OrderViewModel orderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                Order order = new Order
                {
                    OrderNumber = orderViewModel.OrderNumber,
                    CreateTime = DateTime.Now,
                    ApplyTime = DateTime.Parse(orderViewModel.ApplyTime),
                    ApplyUserCode = orderViewModel.ApplyUserCode,
                    ApproveTime = DateTime.Parse(orderViewModel.ApproveTime),
                    ApproveUserCode = orderViewModel.ApproveUserCode,
                    CallingParty = orderViewModel.CallingParty,
                    OrderTypeId = orderViewModel.OrderTypeId
                };
                List<OrderRow> orderRows = new List<OrderRow>();
              
                orderViewModel.OrderRows.ForEach(async(or) =>
                {
                    ReservoirArea area = null;
                    if (or.ReservoirAreaId.HasValue)
                    {
                        ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(or.ReservoirAreaId,null,null,null, null, null);
                        var areas = await this._areaRepository.ListAsync(reservoirAreaSpec);
                        if (areas.Count == 0) throw new Exception(string.Format("子库区[{0}]不存在！",or.ReservoirAreaId));
                        area = areas[0];
                    }
                    OrderRow orderRow = new OrderRow
                    {
                        CreateTime=DateTime.Now,
                        PreCount=or.PreCount,
                        RealityCount=or.PreCount
                    };
                    if (area != null)
                        orderRow.ReservoirAreaId = area.Id;
                });
               var id = await this._orderService.CreateOrder(order);
               response.Data = id;
               await this._logRecordService.AddLog(new LogRecord
               {
                   LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                   LogDesc = string.Format("创建退库订单[{0}]",
                              orderViewModel.OrderNumber),
                   Founder = orderViewModel.Tag?.ToString(),
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

        public async Task<ResponseResultViewModel> SortingOrder(OrderRowViewModel orderRow)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._orderService.SortingOrder(orderRow.OrderId,
                    orderRow.Id, orderRow.Sorting,orderRow.BadCount,
                    orderRow.TrayCode,orderRow.ReservoirAreaId.Value);
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                    LogDesc = string.Format("订单[{0}]出库,订单行[{1}],分拣数量[{2}],分拣托盘[{3}]",
                              orderRow.OrderId,
                              orderRow.Id,
                              orderRow.Sorting,
                              orderRow.TrayCode),
                    Founder = orderRow.Tag?.ToString(),
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

        public async Task<ResponseResultViewModel> OrderOut(OrderRowBatchViewModel orderRowBatchViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._orderService.OrderOut(orderRowBatchViewModel.OrderId.Value,orderRowBatchViewModel.OrderRowId.Value,
                    orderRowBatchViewModel.ReservoirAreaId,orderRowBatchViewModel.BatchCount,orderRowBatchViewModel.Type);
                await this._logRecordService.AddLog(new LogRecord
                {
                    LogType = Convert.ToInt32(LOG_TYPE.操作日志),
                    LogDesc = string.Format("订单[{0}]出库,订单行[{1}],出库批次数量[{2}],出库方式[{3}]",
                                    orderRowBatchViewModel.OrderId,
                                    orderRowBatchViewModel.OrderRowId,
                                    orderRowBatchViewModel.BatchCount,
                                    orderRowBatchViewModel.Type),
                    Founder = orderRowBatchViewModel.Tag?.ToString(),
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
