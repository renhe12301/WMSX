using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Dynamic;

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
                    MaterialUnitId = materialDicViewModel.UnitId,
                    Img = materialDicViewModel.Img,
                    Spec = materialDicViewModel.Spec,
                    Memo = materialDicViewModel.Memo,
                    UpLimit = materialDicViewModel.UpLimit,
                    DownLimit = materialDicViewModel.DownLimit
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
                                             string spec, int? typeId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                if (typeId.HasValue)
                {
                    BaseSpecification<MaterialDicType> baseSpecification = null;

                    if (pageIndex.HasValue && pageIndex >-1 && itemsPage.HasValue && itemsPage > 0)
                    {
                        baseSpecification = new MaterialDicTypePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                            id, typeId, materialCode,materialName,spec);
                    }
                    else
                    {
                        baseSpecification = new MaterialDicTypeSpecification(id, typeId, materialCode, materialName,
                            spec);
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
                            TypeName = e.MaterialType.TypeName,
                            UnitId = e.MaterialDic.MaterialUnitId,
                            UnitName = e.MaterialDic.MaterialUnit.UnitName,
                            CreateTime = e.MaterialDic.CreateTime.ToString(),
                            Spec = e.MaterialDic.Spec,
                            Img = e.MaterialDic.Img,
                            Memo = e.MaterialDic.Memo
                        };
                        materialDicViewModels.Add(materialDicViewModel);
                    });
                    if (pageIndex > -1 && itemsPage > 0)
                    {
                        var count = await this._materialDicTypeRepository.CountAsync(new MaterialDicTypeSpecification(
                            id, typeId, materialCode, materialName,
                            spec));
                        dynamic dyn = new ExpandoObject();
                        dyn.rows = materialDicViewModels;
                        dyn.total = count;
                        response.Data = dyn;
                    }
                    else
                    {
                        response.Data = materialDicViewModels;
                    }
                }
                else
                {
                    BaseSpecification<MaterialDic> baseSpecification = null;

                    if (pageIndex.HasValue && pageIndex >-1 && itemsPage.HasValue && itemsPage > 0)
                    {
                        baseSpecification = new MaterialDicPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                            id,  materialCode,materialName,spec);
                    }
                    else
                    {
                        baseSpecification = new MaterialDicSpecification(id, materialCode, materialName,
                            spec);
                    }
                    var materialDics = await this._materialDicRepository.ListAsync(baseSpecification);
                    List<MaterialDicViewModel> materialDicViewModels = new List<MaterialDicViewModel>();

                    materialDics.ForEach(e =>
                    {
                        MaterialDicViewModel materialDicViewModel = new MaterialDicViewModel
                        {
                            Id = e.Id,
                            MaterialCode = e.MaterialCode,
                            MaterialName = e.MaterialName,
                            UnitId = e.MaterialUnitId,
                            UnitName = e.MaterialUnit?.UnitName,
                            CreateTime = e.CreateTime.ToString(),
                            Spec = e.Spec,
                            Img = e.Img,
                            Memo = e.Memo,
                            UpLimit = e.UpLimit,
                            DownLimit = e.DownLimit
                        };
                        materialDicViewModels.Add(materialDicViewModel);
                    });
                    if (pageIndex > -1&&itemsPage>0)
                    {
                        var count = await this._materialDicRepository.CountAsync(new MaterialDicSpecification(id, materialCode, materialName,
                            spec));
                        dynamic dyn = new ExpandoObject();
                        dyn.rows = materialDicViewModels;
                        dyn.total = count;
                        response.Data = dyn;
                    }
                    else
                    {
                        response.Data = materialDicViewModels;
                    }
                }

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
                    MaterialUnitId = materialDicViewModel.UnitId,
                    Img = materialDicViewModel.Img,
                    Spec = materialDicViewModel.Spec,
                    Memo = materialDicViewModel.Memo,
                    UpLimit = materialDicViewModel.UpLimit,
                    DownLimit = materialDicViewModel.DownLimit
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
