using System;
using Web.Interfaces;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.FlowRecord;
using System.Threading.Tasks;
using Web.ViewModels;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ApplicationCore.Misc;
using Web.ViewModels.FlowRecord;

namespace Web.Services
{
    public class InOutRecordViewModelService:IInOutRecordViewModelService
    {
        private readonly IAsyncRepository<InOutRecord> _inOutRepository;

        public InOutRecordViewModelService(IAsyncRepository<InOutRecord> inOutRepository)
        {
            this._inOutRepository = inOutRepository;
        }

        public async Task<ResponseResultViewModel> GetInOutRecords(int? pageIndex,
            int? itemsPage,string trayCode,int? type,int? ouId,int? wareHouseId, int? areaId,int? orderId,int? orderRowId,string status,
            string sCreateTime, string eCreateTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<InOutRecord> baseSpecification = null;
                List<int> taskStatus = null;
                if (!string.IsNullOrEmpty(status))
                {
                    taskStatus = status.Split(new char[]{
                        ','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

                }
                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new InOutRecordPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        type,ouId,wareHouseId,areaId,orderId,orderRowId,taskStatus,null,sCreateTime,eCreateTime);
                }
                else
                {
                    baseSpecification = new InOutRecordSpecification(trayCode,type,ouId,wareHouseId,areaId,orderId,orderRowId,
                                                                     taskStatus,null,sCreateTime,eCreateTime);
                }
                var inOutRecords = await this._inOutRepository.ListAsync(baseSpecification);
                List<InOutRecordViewModel> inOutRecordViewModels = new List<InOutRecordViewModel>();

                inOutRecords.ForEach(e =>
                {
                    InOutRecordViewModel inOutRecordViewModel = new InOutRecordViewModel
                    {
                        Id = e.Id,
                        CreateTime = e.CreateTime?.ToString(),
                        InOutCount = e.InOutCount,
                        MaterialDicId = e.MaterialDicId,
                        MaterialDicName = e.MaterialDic?.MaterialName,
                        TrayCode = e.TrayCode,
                        OrderId = e.OrderId,
                        OrderRowId = e.OrderRowId,
                        ReservoirAreaId = e.ReservoirAreaId,
                        ReservoirAreaName = e.ReservoirArea?.AreaName,
                        WarehouseName = e.Warehouse?.WhName,
                        Type = e.Type,
                        Status = e.Status,
                        StatusStr = Enum.GetName(typeof(ORDER_STATUS), e.Status)
                    };
                    inOutRecordViewModels.Add(inOutRecordViewModel);
                });
                
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._inOutRepository.CountAsync(new InOutRecordSpecification(trayCode,type,ouId,
                                                      wareHouseId,areaId,orderId,orderRowId,taskStatus,null,sCreateTime,eCreateTime));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = inOutRecordViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = inOutRecordViewModels;
                }
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
