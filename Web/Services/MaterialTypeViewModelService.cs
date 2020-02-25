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
using System.Linq;
using ApplicationCore.Entities.StockManager;

namespace Web.Services
{
    public class MaterialTypeViewModelService: IMaterialTypeViewModelService
    {
        private readonly IMaterialTypeService _materialTypeService;
        private readonly IAsyncRepository<MaterialType> _materialTypeRepository;
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;
        private readonly IAsyncRepository<WarehouseMaterial> _warehouseMaterialRepository;
        public MaterialTypeViewModelService(IMaterialTypeService materialTypeService,
                                            IAsyncRepository<MaterialType> materialTypeRepository,
                                            IAsyncRepository<MaterialDic> materialRepository,
                                            IAsyncRepository<WarehouseMaterial> warehouseMaterialRepository)
        {
            this._materialTypeService = materialTypeService;
            this._materialTypeRepository = materialTypeRepository;
            this._materialDicRepository = materialRepository;
            this._warehouseMaterialRepository = warehouseMaterialRepository;
        }

        public async Task<ResponseResultViewModel> AddMaterialType(MaterialTypeViewModel materialTypeViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                MaterialType materialType = new MaterialType
                {
                    TypeName=materialTypeViewModel.TypeName,
                    CreateTime=DateTime.Now,
                    ParentId = materialTypeViewModel.ParentId
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

        public async Task<ResponseResultViewModel> GetMaterialTypes(int? pageIndex, int? itemsPage,int? id,int? parentId, string typeName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<MaterialType> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
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
                        TypeName = e.TypeName,
                        ParentId =e.ParentId
                    };
                    materialTypeViewModels.Add(materialTypeViewModel);
                });
                materialTypeViewModels.RemoveAll(m => m.ParentId == 0);
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._materialTypeRepository.CountAsync(new MaterialTypeSpecification(id, parentId, typeName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = materialTypeViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = materialTypeViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetMaterialTypeDics(int? pageIndex, int? itemsPage, int? typeId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<MaterialDic> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new MaterialDicPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        null,null,null,typeId,null);
                }
                else
                {
                    baseSpecification = new MaterialDicSpecification(null,null,null,typeId,null);
                }

                var materialDics = await this._materialDicRepository.ListAsync(baseSpecification);
                List<MaterialDicViewModel>  materialDicViewModels = new List<MaterialDicViewModel>();

                materialDics.ForEach(e =>
                {
                    MaterialDicViewModel materialDicViewModel = new MaterialDicViewModel
                    {
                        Id = e.Id,
                        MaterialCode = e.MaterialCode,
                        MaterialName = e.MaterialName,
                        TypeName = e.MaterialType.TypeName,
                        CreateTime = e.CreateTime.ToString(),
                        Img = e.Img,
                        Spec = e.Spec,
                        Memo = e.Memo,
                        UpLimit = e.UpLimit,
                        DownLimit = e.DownLimit,
                        Unit = e.Unit
                    };
                    materialDicViewModels.Add(materialDicViewModel);
                });
                if (pageIndex > -1&&itemsPage>0)
                {
                    var count = await this._materialDicRepository.CountAsync( new MaterialDicSpecification(null,null,null,typeId,null));
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

        public async Task<ResponseResultViewModel> GetMaterialTypeTrees(int rootId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var typeSpec = new MaterialTypeSpecification(null,null, null);
                var types = await this._materialTypeRepository.ListAsync(typeSpec);
                if (types.Count == 0) throw new Exception(string.Format("类型编号{0}不存在", rootId));
                var materialType = types.Find(m=>m.Id==rootId);
                TreeViewModel current = new TreeViewModel
                {
                    Id = materialType.Id,
                    ParentId = materialType.ParentId,
                    Name = materialType.TypeName
                };
                await _MaterialTypeTree(current, current.Children,types);
                response.Data = current;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> MaterialTypeChart(int ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                List<MaterialType> materialTypes = await this._materialTypeRepository.ListAllAsync();
                WarehouseMaterialSpecification warehouseMaterialSpec = new WarehouseMaterialSpecification(null,
                    null,null,null,null,null,null,null,
                    null,null,null,null,ouId,null,null);
                List<WarehouseMaterial> warehouseMaterials = await this._warehouseMaterialRepository.ListAsync(warehouseMaterialSpec);
                List<string> lables = new List<string>();
                List<double> datas = new List<double>();
                var warehouseGroup = warehouseMaterials.GroupBy(w => w.MaterialDic.MaterialTypeId);
                foreach (var wg in warehouseGroup)
                {
                    MaterialType materialType = materialTypes.Find(m => m.Id == wg.Key);
                    lables.Add(materialType.TypeName);
                    double sumCount = wg.Sum(w => w.MaterialCount);
                    datas.Add(sumCount);
                }
               
                dynamic result = new ExpandoObject();
                result.labels = lables;
                result.datas = datas;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        private async Task _MaterialTypeTree(TreeViewModel current, List<TreeViewModel> childs,List<MaterialType> materialTypes)
        {
            var types = materialTypes.FindAll(m=>m.ParentId==current.Id);
            if (types.Count > 0)
                current.Type = "dir";
            else
                current.Type = "leaf";

            types.ForEach(async (type) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = type.Id,
                    ParentId = type.ParentId,
                    Name = type.TypeName
                };
                childs.Add(child);
                await _MaterialTypeTree(child, child.Children,materialTypes);
            });
        }
        
        
    }
}
