using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.OrderManager;

namespace Web.Services
{
    public class SubOrderViewModelService:ISubOrderViewModelService
    {

        private readonly IAsyncRepository<SubOrder> _subOrderRepository;
        private readonly IAsyncRepository<SubOrderRow> _subOrderRowRepository;
        private readonly ISubOrderService _subOrderService;
        private readonly IAsyncRepository<EBSProject> _ebsProjectRepository;
        private readonly IAsyncRepository<EBSTask> _ebsTaskRepository;
        private readonly IAsyncRepository<OU> _ouRepository;

        public SubOrderViewModelService(IAsyncRepository<SubOrder> subOrderRepository,
                                        ISubOrderService subOrderService,
                                        IAsyncRepository<SubOrderRow> subOrderRowRepository,
                                        IAsyncRepository<EBSProject> ebsProjectRepository,
                                        IAsyncRepository<EBSTask> ebsTaskRepository,
                                        IAsyncRepository<OU> ouRepository
            )
        {
            this._subOrderRepository = subOrderRepository;
            this._subOrderService = subOrderService;
            this._subOrderRowRepository = subOrderRowRepository;
            this._ebsProjectRepository = ebsProjectRepository;
            this._ebsTaskRepository = ebsTaskRepository;
            this._ouRepository = ouRepository;
        }

        public async Task<ResponseResultViewModel> GetOrders(int? pageIndex, int? itemsPage, string ids, string orderNumber, 
            int? sourceId,string orderTypeIds,string businessType, string status, int? isBack, int? ouId,int? warehouseId,int? pyId, int? supplierId, string supplierName,
            int? supplierSiteId,string supplierSiteName,string sCreateTime, string eCreateTime, string sFinishTime,string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<SubOrder> baseSpecification = null;
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
                List<int> orderIds = null;
                if (!string.IsNullOrEmpty(ids))
                {
                    orderIds = ids.Split(new char[] { ',' },
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SubOrderPaginatedSpecification(pageIndex.Value, itemsPage.Value, orderIds, orderNumber,sourceId,
                        orderTypes,businessType,orderStatuss,null,null, isBack, ouId,warehouseId,pyId,
                        supplierId,supplierName,supplierSiteId,supplierSiteName,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new SubOrderSpecification(orderIds, orderNumber,sourceId, orderTypes,businessType, orderStatuss, null,
                        null,isBack, ouId, warehouseId, pyId, supplierId, supplierName, supplierSiteId, supplierSiteName,
                        sCreateTime, eCreateTime, sFinishTime, eFinishTime);
                }

                var orders = await this._subOrderRepository.ListAsync(baseSpecification);
                List<SubOrderViewModel> orderViewModels = new List<SubOrderViewModel>();
                orders.ForEach(e =>
                {
                    SubOrderViewModel subOrderViewModel = new SubOrderViewModel
                    {
                        Id = e.Id,
                        OrderNumber = e.OrderNumber,
                        OrderTypeId = e.OrderTypeId,
                        OrderType = e.OrderType?.TypeName,
                        Progress = e.Progress,
                        FinishTime = e.FinishTime?.ToString(),
                        CreateTime = e.CreateTime?.ToString(),
                        WarehouseId = e.WarehouseId,
                        WarehouseName = e.Warehouse?.WhName,
                        OUId = e.OUId,
                        OUName = e.OU?.OUName,
                        SupplierId = e.SupplierId,
                        SupplierName = e.Supplier?.SupplierName,
                        SupplierSiteId = e.SupplierSiteId,
                        SupplierSiteName = e.SupplierSite?.SiteName,
                        Currency = e.Currency,
                        TotalAmount = e.TotalAmount,
                        BusinessTypeCode = e.BusinessTypeCode,
                        Status = e.Status,
                        StatusStr = Enum.GetName(typeof(ORDER_STATUS), e.Status),
                        PhyWarehouseId = e.PhyWarehouseId,
                        PhyWarehouseName = e.PhyWarehouse?.PhyName,
                        IsScrap = e.IsScrap,
                        SourceId = e.SourceId,
                        SourceOrderType = e.SourceOrderType,
                        IsSyncStr = e.IsSync == 1 ? "已同步" : "未同步",
                        OrganizationId = e.OrganizationId,
                        OrganizationName = e.Organization?.OrgName,
                        IsBack = e.IsBack,
                        EmployeeName = e.Employee?.UserName
                    };
                    if (e.EBSProjectId.HasValue)
                    {
                        EBSProjectSpecification eBSProjectSpec = new EBSProjectSpecification(e.EBSProjectId, null, null, null, null, null, null);
                        List<EBSProject> eBSProjects = this._ebsProjectRepository.List(eBSProjectSpec);
                        if (eBSProjects.Count > 0)
                        {
                            subOrderViewModel.EBSProjectId = e.EBSProjectId.GetValueOrDefault();
                            subOrderViewModel.ProjectName = eBSProjects[0].ProjectName;
                        }
                        else 
                        {
                            subOrderViewModel.EBSProjectId = e.EBSProjectId.GetValueOrDefault();
                        }
                    }
                    if (!string.IsNullOrEmpty(e.BusinessTypeCode))
                    {
                        if (e.BusinessTypeCode == "CONSIGNMENT_INVENTORY")
                            subOrderViewModel.BusinessTypeName = "寄售入库";

                        else if (e.BusinessTypeCode == "STORAGE")
                            subOrderViewModel.BusinessTypeName = "采购入库";

                        else
                        {
                            subOrderViewModel.BusinessTypeName = e.BusinessTypeCode;

                        }

                    }
                    orderViewModels.Add(subOrderViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._subOrderRepository.CountAsync(new SubOrderSpecification(orderIds,orderNumber,sourceId,orderTypes,businessType,orderStatuss,null,null, isBack,
                        ouId,warehouseId,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,sCreateTime,eCreateTime,sFinishTime,eFinishTime));
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

        public async Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, string ids, int? subOrderId, int? orderRowId,int? sourceId, string orderTypeIds,
            int? ouId, int? warehouseId, int? reservoirAreaId,string businessType, string ownerType, int? pyId, int? supplierId, string supplierName, int? supplierSiteId,
            string supplierSiteName, string status, string sCreateTime, string eCreateTime, string sFinishTime,
            string eFinishTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<SubOrderRow> baseSpecification = null;
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
                List<int> orderIds = null;
                if (!string.IsNullOrEmpty(ids))
                {
                    orderIds = ids.Split(new char[] { ',' },
                        StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SubOrderRowPaginatedSpecification(pageIndex.Value,itemsPage.Value,orderIds,subOrderId,orderRowId,sourceId,
                        orderTypes,ouId,warehouseId,reservoirAreaId, businessType, ownerType,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime);
                }
                else
                {
                    baseSpecification = new SubOrderRowSpecification(orderIds,subOrderId,orderRowId,sourceId,
                        orderTypes,ouId,warehouseId,reservoirAreaId, businessType, ownerType,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime);
                }
                var orderRows = await this._subOrderRowRepository.ListAsync(baseSpecification);
                List<SubOrderRowViewModel> orderRowViewModels = new List<SubOrderRowViewModel>();
                orderRows.ForEach(e =>
                {
                    SubOrderRowViewModel subOrderRowViewModel = new SubOrderRowViewModel
                    {
                        Id = e.Id,
                        SubOrderId = e.SubOrderId,
                        RowNumber = e.RowNumber,
                        ReservoirAreaId = e.ReservoirAreaId,
                        ReservoirAreaName = e.ReservoirArea?.AreaName,
                        MaterialDicId = e.MaterialDicId,
                        MaterialDicName = e.MaterialDic?.MaterialName,
                        CreateTime = e.CreateTime.ToString(),
                        FinishTime = e.FinishTime.ToString(),
                        PreCount = e.PreCount,
                        Sorting = e.Sorting,
                        RealityCount = e.RealityCount,
                        Progress = e.Progress,
                        Status = e.Status,
                        StatusStr = Enum.GetName(typeof(ORDER_STATUS),e.Status),
                        OrderRowId = e.OrderRowId,
                        Price = e.Price,
                        Amount = e.Amount,
                        UseFor = e.UseFor,
                        IsScrap = e.IsScrap,
                        OUId = e.SubOrder.OUId,
                        WarehouseId = e.SubOrder.WarehouseId,
                        SupplierId = e.SubOrder.SupplierId,
                        SupplierSiteId = e.SubOrder.SupplierSiteId,
                        BusinessTypeCode = e.SubOrder.BusinessTypeCode,
                        SourceId = e.SourceId,
                        OrderTypeId = e.SubOrder?.OrderTypeId,
                        OwnerType = e.OwnerType,
                        ExpenditureType = e.ExpenditureType,
                        Expend = e.Expend.GetValueOrDefault()
                        
                    };
                    if (e.EBSTaskId.HasValue)
                    {
                        EBSTaskSpecification eBSTaskSpec = new EBSTaskSpecification(e.EBSTaskId, null, null, null, null, null, null);
                        List<EBSTask> eBSTasks = this._ebsTaskRepository.List(eBSTaskSpec);
                        if (eBSTasks.Count > 0)
                        {
                            subOrderRowViewModel.EBSTaskId = e.EBSTaskId.GetValueOrDefault();
                            subOrderRowViewModel.EBSTaskName = eBSTasks[0].TaskName;
                        }
                        else 
                        {
                            subOrderRowViewModel.EBSTaskId = e.EBSTaskId.GetValueOrDefault();
                        }
                    }
                    if (e.EBSProjectId.HasValue)
                    {
                        EBSProjectSpecification eBSProjectSpec = new EBSProjectSpecification(e.EBSProjectId, null, null, null, null, null, null);
                        List<EBSProject> eBSProjects = this._ebsProjectRepository.List(eBSProjectSpec);
                        if (eBSProjects.Count > 0)
                        {
                            subOrderRowViewModel.EBSProjectId = e.EBSProjectId.GetValueOrDefault();
                            subOrderRowViewModel.EBSProjectName = eBSProjects[0].ProjectName;
                        }
                        else
                        {
                            subOrderRowViewModel.EBSProjectId = e.EBSProjectId.GetValueOrDefault();
                        }
                    }
                    if (!string.IsNullOrEmpty(e.OwnerType)) 
                    {
                        if (e.OwnerType == "ORDINARY")
                            subOrderRowViewModel.OwnerTypeName = "一般库";

                        if (e.OwnerType == "CONSIGNMENT")
                            subOrderRowViewModel.OwnerTypeName = "寄售库";

                    }
                    orderRowViewModels.Add(subOrderRowViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._subOrderRowRepository.CountAsync(new SubOrderRowSpecification(orderIds,subOrderId,orderRowId,sourceId,
                        orderTypes,ouId,warehouseId,reservoirAreaId, businessType, ownerType,pyId,supplierId,supplierName,supplierSiteId,supplierSiteName,orderStatuss,sCreateTime,
                        eCreateTime,sFinishTime,eFinishTime));
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

        public async Task<ResponseResultViewModel> SortingOrder(int subOrderId, int subOrderRowId, double sortingCount, string trayCode, int areaId, int pyId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._subOrderService.SortingOrder(subOrderId, subOrderRowId, sortingCount, trayCode, areaId, pyId);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResultViewModel> CreateOrder(SubOrderViewModel subOrderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                DateTime now = DateTime.Now;
                SubOrder order = new SubOrder
                {
                    OUId = subOrderViewModel.OUId,
                    WarehouseId = subOrderViewModel.WarehouseId,
                    CreateTime = now,
                    OrderTypeId = subOrderViewModel.OrderTypeId,
                    Status = 0,
                    SupplierId = subOrderViewModel.SupplierId,
                    SupplierSiteId = subOrderViewModel.SupplierSiteId,
                    Currency = subOrderViewModel.Currency,
                    BusinessTypeCode = subOrderViewModel.BusinessTypeCode,
                    TotalAmount = subOrderViewModel.TotalAmount,
                    SourceId = subOrderViewModel.SourceId,
                    SourceOrderType = subOrderViewModel.SourceOrderType,
                    OrganizationId = subOrderViewModel.OrganizationId,
                    EmployeeId = subOrderViewModel.EmployeeId,
                    EBSProjectId = subOrderViewModel.EBSProjectId,
                    IsBack = 1
                };

                string code = await this._BuildCode(subOrderViewModel.OUId, subOrderViewModel.OrderTypeId);
                order.OrderNumber = code;

                List<SubOrderRow> subOrderRows = new List<SubOrderRow>();
                int index = 1;
                subOrderViewModel.SubOrderRows.ForEach(async(or) =>
                {
                    SubOrderRow orderRow = new SubOrderRow
                    {
                        CreateTime=now,
                        PreCount=or.PreCount,
                        ReservoirAreaId = or.ReservoirAreaId,
                        MaterialDicId = or.MaterialDicId,
                        Price = or.Price,
                        Status = 0,
                        OrderRowId = or.OrderRowId,
                        RowNumber = index.ToString(),
                        UseFor = or.UseFor,
                        Amount = or.Amount,
                        SourceId = or.SourceId,
                        ExpenditureType = or.ExpenditureType,
                        OwnerId = or.OwnerId,
                        OwnerType = or.OwnerType,
                        EBSTaskId = or.EBSTaskId,
                        EBSProjectId=or.EBSProjectId
                        
                    };
                    index++;
                    subOrderRows.Add(orderRow);
                });
                order.SubOrderRow = subOrderRows;
                await  this._subOrderService.CreateOrder(order);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("警告"))
                {
                    response.Code = 200;
                    response.Data = ex.Message;
                }
                else 
                {
                    response.Code = 500;
                    response.Data = ex.Message;
                }
              
            }
            return response;
        }

        private async Task<string> _BuildCode(int ouId,int orderTypeId) 
        {
            string code = "";
            OUSpecification oUSpecification = new OUSpecification(ouId, null, null, null);
            List<OU> oUs = await this._ouRepository.ListAsync(oUSpecification);
            if (oUs.Count() == 0) throw new Exception(string.Format("业务实体Id[{0}]不存在!", ouId));
            code = oUs[0].OUCode;
            code += "W";
            if (orderTypeId == Convert.ToInt32(ORDER_TYPE.入库接收))
                code += "RK";
            if (orderTypeId == Convert.ToInt32(ORDER_TYPE.出库领料))
                code += "CK";
            if (orderTypeId == Convert.ToInt32(ORDER_TYPE.入库退料))
                code += "TK";
            if (orderTypeId == Convert.ToInt32(ORDER_TYPE.出库退库))
                code += "RT";
            code += DateTime.Now.Year;
            string sCreateTime = DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1).ToShortDateString() + " 00:00:00";
            string eCreateTime = DateTime.Now.ToString();
            SubOrderSpecification subOrderSpec = new SubOrderSpecification(null, null, null, new List<int> { orderTypeId }, null, null, null,
                null, null, ouId, null, null, null, null, null, null, sCreateTime, eCreateTime, null, null);
            List<SubOrder> subOrders = await this._subOrderRepository.ListAsync(subOrderSpec);
            if (subOrders.Count > 0)
                code += subOrders.Count.ToString().PadLeft(6, '0');
            else
                code += "1".PadLeft(6, '0');
            return code;
        }

        public async Task<ResponseResultViewModel> ScrapOrder(SubOrderViewModel subOrderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                SubOrder order = new SubOrder
                {
                    Id = subOrderViewModel.Id
                };
                await  this._subOrderService.ScrapOrder(order);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> ScrapOrderRow(List<SubOrderRowViewModel> subOrderRowViewModels)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                List<SubOrderRow> orderRows = new List<SubOrderRow>();
                subOrderRowViewModels.ForEach(async(or) =>
                {
                    SubOrderRow orderRow = new SubOrderRow
                    {
                        Id=or.Id,
                        OrderRowId = or.OrderRowId
                    };
                    orderRows.Add(orderRow);
                });
                await  this._subOrderService.ScrapOrderRow(orderRows);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> OutConfirm(int subOrderId,int pyId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                await  this._subOrderService.OutConfirm(subOrderId,pyId);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> CreateTKOrder(SubOrderViewModel subOrderViewModel)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                DateTime now = DateTime.Now;
                SubOrder order = new SubOrder
                {
                    Id = subOrderViewModel.Id,
                    OUId = subOrderViewModel.OUId,
                    WarehouseId = subOrderViewModel.WarehouseId,
                    CreateTime = now,
                    OrderTypeId = subOrderViewModel.OrderTypeId,
                    Status = 0,
                    SupplierId = subOrderViewModel.SupplierId,
                    SupplierSiteId = subOrderViewModel.SupplierSiteId,
                    Currency = subOrderViewModel.Currency,
                    BusinessTypeCode = subOrderViewModel.BusinessTypeCode,
                    TotalAmount = subOrderViewModel.SubOrderRows.Sum(r => r.Price).GetValueOrDefault() * subOrderViewModel.SubOrderRows.Sum(r => r.PreCount),
                    SourceId = subOrderViewModel.SourceId,
                    SourceOrderType = subOrderViewModel.SourceOrderType,
                    OrganizationId = subOrderViewModel.OrganizationId,
                    EmployeeId = subOrderViewModel.EmployeeId,
                    EBSProjectId = subOrderViewModel.EBSProjectId

                };
                string code = await this._BuildCode(subOrderViewModel.OUId, subOrderViewModel.OrderTypeId);
                order.OrderNumber = code;
                List<SubOrderRow> subOrderRows = new List<SubOrderRow>();
                int index = 1;
                subOrderViewModel.SubOrderRows.ForEach(async (or) =>
                {
                    SubOrderRow orderRow = new SubOrderRow
                    {
                        Id = or.Id,
                        CreateTime = now,
                        PreCount = or.PreCount,
                        ReservoirAreaId = or.ReservoirAreaId,
                        MaterialDicId = or.MaterialDicId,
                        Price = or.Price,
                        Status = 0,
                        OrderRowId = or.OrderRowId,
                        RowNumber = index.ToString(),
                        UseFor = or.UseFor,
                        Amount = or.PreCount * or.Price,
                        SourceId = or.SourceId,
                        ExpenditureType = or.ExpenditureType,
                        OwnerId = or.OwnerId,
                        OwnerType = or.OwnerType,
                        EBSTaskId = or.EBSTaskId,
                        EBSProjectId = or.EBSProjectId

                    };
                    index++;
                    subOrderRows.Add(orderRow);
                });
                order.SubOrderRow = subOrderRows;
                await this._subOrderService.CreateTKOrder(order);
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