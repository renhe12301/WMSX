﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;

namespace Web.Interfaces
{
    public interface IReservoirAreaViewModelService
    {
        Task<ResponseResultViewModel> AssignLocation(LocationViewModel locationViewModel);
        Task<ResponseResultViewModel> GetAreas(int? pageIndex, int? itemsPage, int? id, int? ouId,int? wareHouseId,string ownerType, string areaName);
        Task<ResponseResultViewModel> SetOwnerType(ReservoirAreaViewModel reservoirAreaViewModel);


    }
}
