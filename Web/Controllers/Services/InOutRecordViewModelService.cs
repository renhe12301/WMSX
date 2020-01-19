using System;
using Web.Interfaces;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.FlowRecord;
using System.Threading.Tasks;
using Web.ViewModels;
using ApplicationCore.Specifications;
using System.Collections.Generic;
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

        public async Task<ResponseResultViewModel> GetInOutRecords(int? pageIndex, int? itemsPage,
            int? type,string sCreateTime,string eCreateTime)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<InOutRecord> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new InOutRecordPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        type,sCreateTime,eCreateTime);
                }
                else
                {
                    baseSpecification = new InOutRecordSpecification(type,null,sCreateTime,eCreateTime);
                }
                var inOutRecords = await this._inOutRepository.ListAsync(baseSpecification);
                List<InOutRecordViewModel> inOutRecordViewModels = new List<InOutRecordViewModel>();

                inOutRecords.ForEach(e =>
                {
                    InOutRecordViewModel inOutRecordViewModel = new InOutRecordViewModel
                    {
                        CreateTime = e.CreateTime.ToString(),
                        InOutCount = e.InOutCount,
                        MaterialDicId = e.MaterialDicId,
                        MaterialDicName = e.MaterialDic.MaterialName,
                        TrayDicName = e.TrayDic.TrayName,
                        OrderId = e.OrderId,
                        OrderRowId = e.OrderRowId,
                        ReservoirAreaId = e.ReservoirAreaId,
                        ReservoirAreaName = e.ReservoirArea.AreaName,
                        WarehouseName = e.Warehouse.WhName,
                        Type = e.Type
                    };
                    inOutRecordViewModels.Add(inOutRecordViewModel);
                });
                response.Data = inOutRecordViewModels;
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
