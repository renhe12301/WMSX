using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using System.Collections.Generic;
using ApplicationCore.Specifications;

namespace Web.Services
{
    public class TrayTypeViewModelService:ITrayTypeViewModelService
    {
        private readonly IAsyncRepository<TrayType> _trayTypeRepository;
        private readonly ITrayTypeService _trayTypeService;

        public TrayTypeViewModelService(IAsyncRepository<TrayType> trayTypeRepository,
                                        ITrayTypeService trayTypeService)
        {
            this._trayTypeRepository = trayTypeRepository;
            this._trayTypeService = trayTypeService;

        }

        public async Task<ResponseResultViewModel> AddTrayType(TrayTypeViewModel trayTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code=200};
            try
            {
                TrayType trayType = new TrayType
                {
                    TypeName = trayTypeViewModel.TypeName,
                    CreateTime = DateTime.Now,
                    ParentId = trayTypeViewModel.ParentId
                };
                await this._trayTypeService.AddTrayType(trayType);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> AssignTrayDic(TrayTypeViewModel trayTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                List<int> childIds = trayTypeViewModel.DicChilds.ConvertAll(c => c.Id);
                await this._trayTypeService.AssignTrayDic(trayTypeViewModel.Id, childIds);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> DelTrayType(TrayTypeViewModel trayTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };

            try
            {
                await this._trayTypeService.DelTrayType(trayTypeViewModel.Id);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> GetTrayTypes(int? pageIndex, int? itemsPage, int? id, int? parentId, string typeName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<TrayType> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new TrayTypePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, parentId, typeName);
                }
                else
                {
                    baseSpecification = new TrayTypeSpecification(id, parentId, typeName);
                }

                var trayDicTypes = await this._trayTypeRepository.ListAsync(baseSpecification);
                List<TrayTypeViewModel> trayTypeViewModels = new List<TrayTypeViewModel>();

                trayDicTypes.ForEach(e =>
                {
                    TrayTypeViewModel trayTypeViewModel = new TrayTypeViewModel
                    {
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        Memo = e.Memo,
                        TypeName = e.TypeName
                    };
                    trayTypeViewModels.Add(trayTypeViewModel);
                });
                response.Data = trayTypeViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetTrayTypeTree(int rootId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var typeSpec = new TrayTypeSpecification(rootId, null, null);
                var types = await this._trayTypeRepository.ListAsync(typeSpec);
                if (types.Count == 0) throw new Exception(string.Format("类型编号{0}不存在", rootId));
                var type = types[0];
                TrayTypeViewModel current = new TrayTypeViewModel
                {
                    Id = type.Id,
                    TypeName = type.TypeName
                };
                await _TrayTypeTree(current, current.TypeChilds);
                response.Data = current;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        private async Task _TrayTypeTree(TrayTypeViewModel current, List<TrayTypeViewModel> childs)
        {
            var typeSpec = new TrayTypeSpecification(null, current.ParentId, null);
            var types = await this._trayTypeRepository.ListAsync(typeSpec);
            types.ForEach(async (t) =>
            {
                TrayTypeViewModel child = new TrayTypeViewModel
                {
                    Id = t.Id,
                    ParentId = t.ParentId,
                    TypeName = t.TypeName
                };
                childs.Add(child);
                await _TrayTypeTree(child, child.TypeChilds);
            });
        }

        public async Task<ResponseResultViewModel> UpdateTrayType(TrayTypeViewModel trayTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                TrayType trayType = new TrayType
                {
                    TypeName = trayTypeViewModel.TypeName,
                    ParentId = trayTypeViewModel.ParentId
                };
                await this._trayTypeService.UpdateTrayType(trayType);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }
    }
}
