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
    public class MaterialDicViewModelService : IMaterialDicViewModelService
    {

        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;
        private readonly IAsyncRepository<MaterialDicType> _materialDicTypeRepository;

        private readonly IMaterialDicService _materialDicService;

        public MaterialDicViewModelService(IAsyncRepository<MaterialDic> materialDicRepository,
                                           IMaterialDicService materialDicService,
                                           IAsyncRepository<MaterialDicType> materialDicTypeRepository)
        {
            this._materialDicRepository = materialDicRepository;
            this._materialDicService = materialDicService;
            this._materialDicTypeRepository = materialDicTypeRepository;
        }

        public async Task<ResponseResultViewModel> AddMaterialDic(MaterialDicViewModel materialDicViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                MaterialDic materialDic = new MaterialDic
                {
                    MaterialCode = materialDicViewModel.MaterialCode,
                    MaterialName = materialDicViewModel.MaterialName,
                    CreateTime = DateTime.Now,
                    UnitId = materialDicViewModel.UnitId,
                    Img = materialDicViewModel.Img,
                    Spec = materialDicViewModel.Spec,
                    Memo = materialDicViewModel.Memo
                };
                await this._materialDicService.AddMaterialDic(materialDic);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> DelMaterialDic(MaterialDicViewModel materialDicViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._materialDicService.DelMaterialDic(materialDicViewModel.Id);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> GetMaterialDics(int? pageIndex, int? itemsPage,
                                             int? id, string materialCode, string materialName,
                                             string spec, int? typeId, int? unitId, int? upLimit,
                                             int? downLimit)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<MaterialDicType> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new MaterialDicTypePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, typeId, materialCode,materialName,spec,unitId,upLimit,downLimit);
                }
                else
                {
                    baseSpecification = new MaterialDicTypeSpecification(id,typeId, materialCode, materialName,
                                            spec, unitId);
                }
                var materialDicTypes = await this._materialDicTypeRepository.ListAsync(baseSpecification);
                List<MaterialDicViewModel> materialDicViewModels = new List<MaterialDicViewModel>();

                materialDicTypes.ForEach(e =>
                {
                    MaterialDicViewModel materialDicViewModel = new MaterialDicViewModel
                    {
                        Id = e.Id,
                        MaterialCode = e.MaterialDic.MaterialCode,
                        MaterialName = e.MaterialDic.MaterialName,
                        TypeId = e.MaterialType.Id,
                        TypeName = e.MaterialType.TypeName,
                        UnitId = e.MaterialDic.UnitId,
                        UnitName = e.MaterialDic.MaterialUnit.UnitName,
                        CreateTime = e.MaterialDic.CreateTime.ToString(),
                        Spec = e.MaterialDic.Spec,
                        Img = e.MaterialDic.Img,
                        Memo = e.MaterialDic.Memo
                    };
                    materialDicViewModels.Add(materialDicViewModel);
                });
                response.Data = materialDicViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }
    

        public async Task<ResponseResultViewModel> UpdateMaterialDic(MaterialDicViewModel materialDicViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                MaterialDic materialDic = new MaterialDic
                {
                    Id=materialDicViewModel.Id,
                    MaterialCode = materialDicViewModel.MaterialCode,
                    MaterialName = materialDicViewModel.MaterialName,
                    CreateTime = DateTime.Now,
                    UnitId = materialDicViewModel.UnitId,
                    Img = materialDicViewModel.Img,
                    Spec = materialDicViewModel.Spec,
                    Memo = materialDicViewModel.Memo
                };
                await this._materialDicService.UpdateMaterialDic(materialDic);
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
