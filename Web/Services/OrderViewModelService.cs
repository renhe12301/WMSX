﻿using System;
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
        
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<ReservoirArea> _areaRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly ILogRecordService _logRecordService;
        private readonly IAsyncRepository<OrderRow> _orderRowRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;

        public OrderViewModelService(  IAsyncRepository<Order> orderRepository,
                                       IAsyncRepository<ReservoirArea> areaRepository,
                                       IAsyncRepository<Warehouse> warehouseRepository,
                                       ILogRecordService logRecordService,
                                       IAsyncRepository<OrderRow> orderRowRepository,
                                       IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository)
        {
            this._orderRepository = orderRepository;
            this._areaRepository = areaRepository;
            this._warehouseRepository = warehouseRepository;
            this._logRecordService = logRecordService;
            this._orderRowRepository = orderRowRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
        }
        
        public async Task<ResponseResultViewModel> GetOrders(int? pageIndex,int? itemsPage,
            int?id,string orderNumber,int? sourceId, int? orderTypeId,string status,int? ouId,int? warehouseId,int? pyId,string applyUserCode, 
            string approveUserCode,int? employeeId,string employeeName, int? supplierId, string supplierName, string sApplyTime, string eApplyTime, string sApproveTime,
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
                    spec = new OrderPaginatedSpecification(pageIndex.Value,itemsPage.Value,id, orderNumber,sourceId, orderTypeId,
                        orderStatuss,ouId,warehouseId,pyId, applyUserCode, approveUserCode,employeeId,employeeName, supplierId, supplierName, 
                        sApplyTime, eApplyTime, sApproveTime, eApproveTime,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }
                else
                {
                    spec = new OrderSpecification(id, orderNumber,sourceId, orderTypeId,
                        orderStatuss,ouId,warehouseId,pyId,applyUserCode, approveUserCode, employeeId,employeeName, supplierId, supplierName,sApplyTime, eApplyTime, 
                        sApproveTime, eApproveTime,sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }
                var orders = await this._orderRepository.ListAsync(spec);
                List<OrderViewModel> orderViewModels = new List<OrderViewModel>();

                orders.ForEach(e =>
                {
                    OrderViewModel orderViewModel = new OrderViewModel();
                    orderViewModel.Id = e.Id;
                    orderViewModel.OrderNumber = e.OrderNumber;
                    orderViewModel.OrderType = e.OrderType?.TypeName;
                    orderViewModel.OrderTypeId = e.OrderTypeId;
                    orderViewModel.ApplyUserCode = e.ApplyUserCode;
                    orderViewModel.ApproveUserCode = e.ApproveUserCode;
                    orderViewModel.ApplyTime = e.ApplyTime?.ToString();
                    orderViewModel.ApproveTime = e.ApproveTime?.ToString();
                    orderViewModel.CallingParty = e.CallingParty;
                    orderViewModel.Progress = e.Progress;
                    orderViewModel.Memo = e.Memo;
                    orderViewModel.CreateTime = e.CreateTime?.ToString();
                    orderViewModel.FinishTime = e.FinishTime?.ToString();
                    orderViewModel.WarehouseId = e.WarehouseId;
                    orderViewModel.WarehouseName = e.Warehouse?.WhName;
                    orderViewModel.OUName = e.OU?.OUName;
                    orderViewModel.OUId = e.OUId;
                    // EBSProjectId = e.EBSProjectId.GetValueOrDefault(),
                    // ProjectName = e.EBSProject?.ProjectName,
                    orderViewModel.SupplierId = e.SupplierId.GetValueOrDefault();
                    orderViewModel.SupplierName = e.Supplier?.SupplierName;
                    orderViewModel.SupplierSiteId = e.SupplierSiteId.GetValueOrDefault();
                    orderViewModel.SupplierSiteName = e.SupplierSite?.SiteName;
                    orderViewModel.Currency = e.Currency;
                    orderViewModel.TotalAmount = e.TotalAmount;
                    orderViewModel.Status = e.Status;
                    //orderViewModel.StatusStr = Enum.GetName(typeof(ORDER_STATUS), e.Status);
                    orderViewModel.EmployeeId = e.EmployeeId;
                    orderViewModel.EmployeeName = e.Employee?.UserName;
                    
                    orderViewModels.Add(orderViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._orderRepository.CountAsync(new OrderSpecification(id, orderNumber,sourceId, orderTypeId,
                        orderStatuss,ouId,warehouseId, pyId,applyUserCode, approveUserCode, employeeId,employeeName,supplierId, supplierName,
                        sApplyTime, eApplyTime, sApproveTime, eApproveTime,sCreateTime, eCreateTime, sFinishTime, eFinishTime));
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

        public async Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, int? id, int? orderId,string orderTypeIds,int? sourceId, 
            string orderNumber, int? ouId,int? warehouseId,int? supplierId, string supplierName,int? supplierSiteId, string supplierSiteName,string status,
            string sCreateTime, string eCreateTime, string sFinishTime, string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<OrderRow> spec = null;
                List<int> orderStatuss = null;
                if (!string.IsNullOrEmpty(status))
                {
                    orderStatuss = status.Split(new char[]{','}, 
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                List<int> orderTypes = null;
                if (!string.IsNullOrEmpty(orderTypeIds))
                {
                    orderTypes = orderTypeIds.Split(new char[]{','}, 
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    spec = new OrderRowPaginatedSpecification(pageIndex.Value,itemsPage.Value,id,orderId,orderTypes,sourceId,
                        orderNumber,ouId,warehouseId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }
                else
                {
                    spec = new OrderRowSpecification(id,orderId,orderTypes,sourceId,
                        orderNumber,ouId,warehouseId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }
                var orderRows = await this._orderRowRepository.ListAsync(spec);
                List<OrderRowViewModel> orderRowViewModels = new List<OrderRowViewModel>();
                
                orderRows.ForEach(e =>
                {
                    OrderRowViewModel orderRowViewModel = new OrderRowViewModel
                    {
                        Id = e.Id,
                        RowNumber = e.RowNumber,
                        ReservoirAreaId = e.ReservoirAreaId,
                        ReservoirAreaName = e.ReservoirArea?.AreaName,
                        MaterialDicId = e.MaterialDicId,
                        MaterialDicName = e.MaterialDic?.MaterialName,
                        CreateTime = e.CreateTime.ToString(),
                        FinishTime = e.FinishTime?.ToString(),
                        PreCount = e.PreCount,
                        RealityCount = e.RealityCount.GetValueOrDefault(),
                        Sorting = e.Sorting.GetValueOrDefault(),
                        Status = e.Status,
                        UseFor = e.UseFor,
                        //StatusStr = Enum.GetName(typeof(ORDER_STATUS), e.Status),
                        Progress = e.Progress.GetValueOrDefault(),
                        //EBSTaskName = e.EBSTask?.TaskName,
                        Price = e.Price,
                        Amount = e.Amount,
                        OrderId = e.OrderId,
                        RelatedId = e.RelatedId

                    };
                    orderRowViewModels.Add(orderRowViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._orderRowRepository.CountAsync(new OrderRowSpecification(id,orderId,orderTypes,sourceId,
                        orderNumber,ouId,warehouseId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = orderRowViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = orderRowViewModels;
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
                OrderRowSpecification orderRowSpec = new OrderRowSpecification(null,null,null,null,null,
                    null,null,null,null,null,null,
                     new List<int>{Convert.ToInt32(ORDER_STATUS.待处理),Convert.ToInt32(ORDER_STATUS.执行中)},null,
                     null,null,null);
                 List<OrderRow> orderRows = await this._orderRowRepository.ListAsync(orderRowSpec);
                 List<OrderRow> tkOrderRows = orderRows.Where(or => or.Order.OrderTypeId == Convert.ToInt32(ORDER_TYPE.入库退库)).ToList();
                 ReservoirAreaSpecification reservoirAreaSpec = new ReservoirAreaSpecification(areaId,null,ouId,warehouseId,null,null);
                 List<ReservoirArea> areas = await this._areaRepository.ListAsync(reservoirAreaSpec);
                 
                 WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,null,
                     null,null,null,null,null,null,null,
                     null,new List<int>(){Convert.ToInt32(TRAY_STEP.入库完成),Convert.ToInt32(TRAY_STEP.初始化)},
                     null,ouId,warehouseId,null,null,null,null,null,null);
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
                         double totalAmount = mg.Sum(m => m.Amount.GetValueOrDefault());
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
        
    }
}
