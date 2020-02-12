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

        private readonly IMaterialDicService _materialDicService;

        public MaterialDicViewModelService(IAsyncRepository<MaterialDic> materialDicRepository,
                                           IMaterialDicService materialDicService)
        {
            this._materialDicRepository = materialDicRepository;
            this._materialDicService = materialDicService;
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
                    Unit = materialDicViewModel.Unit,
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

        public async Task<ResponseResultViewModel> GetMaterialDics(int? pageIndex, int? itemsPage,
                                             int? id, string materialCode, string materialName,
                                             string spec, int? typeId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<MaterialDic> baseSpecification = null;

                    if (pageIndex.HasValue && pageIndex >-1 && itemsPage.HasValue && itemsPage > 0)
                    {
                        baseSpecification = new MaterialDicPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                            id,materialCode,materialName,typeId,spec);
                    }
                    else
                    {
                        baseSpecification = new MaterialDicSpecification(id, materialCode, materialName,
                            typeId,spec);
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
                            Unit = e.Unit,
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
                            typeId,spec));
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
                    Unit = materialDicViewModel.Unit,
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
