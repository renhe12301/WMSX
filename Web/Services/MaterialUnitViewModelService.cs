using System;
using Web.ViewModels;
using Web.Interfaces;
using System.Threading.Tasks;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Specifications;
using System.Collections.Generic;

namespace Web.Services
{
    public class MaterialUnitViewModelService : IMaterialUnitViewModelService
    {

        private readonly IMaterialUnitService _materialUnitService;
        private readonly IAsyncRepository<MaterialUnit> _materialUnitRepository;

        public MaterialUnitViewModelService(IMaterialUnitService materialUnitServic,
                                             IAsyncRepository<MaterialUnit> materialUnitRepository)
        {
            this._materialUnitService = materialUnitServic;
            this._materialUnitRepository = materialUnitRepository;
        }

        public async Task<ResponseResultViewModel> AddMaterialUnit(MaterialUnitViewModel materialUnitViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                MaterialUnit materialUnit = new MaterialUnit
                {
                    UnitName = materialUnitViewModel.UnitName,
                    CreateTime = DateTime.Now,
                    Memo = materialUnitViewModel.Memo
                };
                await this._materialUnitService.AddMaterialUnit(materialUnit);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> DelMaterialUnit(MaterialUnitViewModel materialUnitViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                await this._materialUnitService.DelMaterialUnit(materialUnitViewModel.Id);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> GetMaterialUnits(int? pageIndex, int? itemsPage,int? id, string unitName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<MaterialUnit> baseSpecification = null;
                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new MaterialUnitPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, unitName);
                }
                else
                {
                    baseSpecification = new MaterialUnitPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, unitName);
                }

                var materialUnits = await this._materialUnitRepository.ListAsync(baseSpecification);
                List<MaterialUnitViewModel> materialUnitViewModels = new List<MaterialUnitViewModel>();

                materialUnits.ForEach(e =>
                {
                    MaterialUnitViewModel materialUnitViewModel = new MaterialUnitViewModel
                    {
                        Id = e.Id,
                        CreateTime = e.CreateTime.ToString(),
                        Memo = e.Memo,
                        UnitName = e.UnitName
                    };
                    materialUnitViewModels.Add(materialUnitViewModel);
                });
                response.Data = materialUnitViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> UpdateMaterialUnit(MaterialUnitViewModel materialUnitViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                MaterialUnit materialUnit = new MaterialUnit
                {
                    UnitName = materialUnitViewModel.UnitName,
                    CreateTime = DateTime.Now,
                    Memo = materialUnitViewModel.Memo
                };
                await this._materialUnitService.UpdateMaterialUnit(materialUnit);
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
