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
    public class MaterialTypeViewModelService: IMaterialTypeViewModelService
    {
        private readonly IMaterialTypeService _materialTypeService;
        private readonly IAsyncRepository<MaterialType> _materialTypeRepository;

        public MaterialTypeViewModelService(IMaterialTypeService materialTypeService,
                                            IAsyncRepository<MaterialType> materialTypeRepository)
        {
            this._materialTypeService = materialTypeService;
            this._materialTypeRepository = materialTypeRepository;
        }

        public async Task<ResponseResultViewModel> AddMaterialType(MaterialTypeViewModel materialTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                MaterialType materialType = new MaterialType
                {
                    TypeName=materialTypeViewModel.TypeName,
                    CreateTime=DateTime.Now
                };
                await this._materialTypeService.AddMaterialType(materialType);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> DelMaterialType(MaterialTypeViewModel materialTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._materialTypeService.DelMaterialType(materialTypeViewModel.Id);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> GetMaterialTypes(int? pageIndex, int? itemsPage,int? id,int? parentId, string typeName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<MaterialType> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new MaterialTypePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, parentId, typeName);
                }
                else
                {
                    baseSpecification = new MaterialTypeSpecification(id, parentId, typeName);
                }

                var materialDicTypes = await this._materialTypeRepository.ListAsync(baseSpecification);
                List<MaterialTypeViewModel> materialTypeViewModels = new List<MaterialTypeViewModel>();

                materialDicTypes.ForEach(e =>
                {
                    MaterialTypeViewModel materialTypeViewModel = new MaterialTypeViewModel
                    {
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        Memo = e.Memo,
                        TypeName = e.TypeName
                    };
                    materialTypeViewModels.Add(materialTypeViewModel);
                });
                response.Data = materialTypeViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetMaterialTypeTree(int rootId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var typeSpec = new MaterialTypeSpecification(rootId,null, null);
                var types = await this._materialTypeRepository.ListAsync(typeSpec);
                if (types.Count == 0) throw new Exception(string.Format("类型编号{0}不存在", rootId));
                var materialType = types[0];
                MaterialTypeViewModel current = new MaterialTypeViewModel
                {
                    Id = materialType.Id,
                    TypeName = materialType.TypeName
                };
                await _MaterialTypeTree(current, current.TypeChilds);
                response.Data = current;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        private async Task _MaterialTypeTree(MaterialTypeViewModel current, List<MaterialTypeViewModel> childs)
        {
            var typeSpec = new MaterialTypeSpecification(null, current.ParentId, null);
            var types = await this._materialTypeRepository.ListAsync(typeSpec);
            types.ForEach(async (org) =>
            {
                MaterialTypeViewModel child = new MaterialTypeViewModel
                {
                    Id = org.Id,
                    ParentId = org.ParentId,
                    TypeName = org.TypeName
                };
                childs.Add(child);
                await _MaterialTypeTree(child, child.TypeChilds);
            });
        }

        public async Task<ResponseResultViewModel> UpdateMaterialType(MaterialTypeViewModel materialTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                MaterialType materialType = new MaterialType
                {
                    TypeName = materialTypeViewModel.TypeName,
                    CreateTime = DateTime.Now
                };
                await this._materialTypeService.UpdateMaterialType(materialType);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> AssignMaterialDic(MaterialTypeViewModel materialTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                List<int> materialIds = materialTypeViewModel.DicChilds.ConvertAll(c => c.Id);
                await this._materialTypeService.AssignMaterialDic(materialTypeViewModel.Id,materialIds);
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
