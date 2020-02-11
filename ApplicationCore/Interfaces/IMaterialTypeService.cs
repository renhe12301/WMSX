using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IMaterialTypeService
    {
        Task AddMaterialType(MaterialType materialType);
    }
}
