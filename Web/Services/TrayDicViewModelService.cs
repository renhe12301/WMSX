using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Specifications;
using System.Collections.Generic;

namespace Web.Services
{
    public class TrayDicViewModelService : ITrayDicViewModelService
    {
        private readonly ITrayDicService _trayDicService;
        private readonly IAsyncRepository<TrayDic> _trayDicRepository;
        private readonly IAsyncRepository<TrayDicType> _trayDicTypeRepository;

        

        public TrayDicViewModelService(IAsyncRepository<TrayDic> trayDicRepository,
                                        ITrayDicService trayDicService,
                                        IAsyncRepository<TrayDicType> trayDicTypeRepository)
        {
            this._trayDicRepository = trayDicRepository;
            this._trayDicService = trayDicService;
            this._trayDicTypeRepository = trayDicTypeRepository;
        }

        public async Task<ResponseResultViewModel> GetTrayDics(int? pageIndex, int? itemsPage,
                                                     int? id, string trayCode,
                                                     string trayName,
                                                     int? typeId)
        {

            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<TrayDicType> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new TrayDicTypePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, typeId,trayCode,trayName);
                }
                else
                {
                    baseSpecification = new TrayDicTypeSpecification(id, typeId,trayCode,trayName);
                }
                var trayDicTypes = await this._trayDicTypeRepository.ListAsync(baseSpecification);
                List<TrayDicViewModel> trayDicViewModels = new List<TrayDicViewModel>();

                trayDicTypes.ForEach(e =>
                {
                    TrayDicViewModel trayDicViewModel = new TrayDicViewModel
                    {
                        Id = e.Id,
                        TrayCode = e.TrayDic.TrayCode,
                        TrayName = e.TrayType.TypeName,
                        TypeId = e.TrayType.Id,
                        CreateTime = e.TrayDic.CreateTime.ToString(),
                        Memo = e.TrayDic.Memo
                    };
                    trayDicViewModels.Add(trayDicViewModel);
                });
                response.Data = trayDicViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> AddTrayDic(TrayDicViewModel trayDicViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                TrayDic trayDic = new TrayDic
                {
                    TrayCode=trayDicViewModel.TrayCode,
                    TrayName=trayDicViewModel.TrayName,
                    CreateTime=DateTime.Now
                };
                await this._trayDicService.AddTrayDic(trayDic);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 200;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> DelTrayDic(TrayDicViewModel trayDicViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._trayDicService.DelTrayDic(trayDicViewModel.Id);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 200;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> UpdateTrayDic(TrayDicViewModel trayDicViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                TrayDic trayDic = new TrayDic
                {
                    Id=trayDicViewModel.Id,
                    TrayName = trayDicViewModel.TrayName
                };
                await this._trayDicService.UpdateTrayDic(trayDic);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 200;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }
    }
}
