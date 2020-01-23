using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IReservoirAreaViewModelService
    {
        Task<ResponseResultViewModel> AddArea(ReservoirAreaViewModel reservoirAreaViewModel);
        Task<ResponseResultViewModel> UpdateArea(ReservoirAreaViewModel reservoirAreaViewModel);
        Task<ResponseResultViewModel> Enable(ReservoirAreaViewModel reservoirAreaViewModel);
        Task<ResponseResultViewModel> Disable(ReservoirAreaViewModel reservoirAreaViewModel);
        Task<ResponseResultViewModel> AssignLocation(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> AssignMaterialType(MaterialDicTypeAreaViewModel materialDicTypeAreaViewModel);
        Task<ResponseResultViewModel> GetAreas(int? pageIndex, int? itemsPage, int? id, int? orgId,int? ouId,int? wareHouseId,int? type, string areaName);
        Task<ResponseResultViewModel> GetMaterialTypes(int? pageIndex, int? itemsPage, int? areaId);
    }
}
