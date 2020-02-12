using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IMaterialTypeService
    {
        Task AddMaterialType(MaterialType materialType,bool unique=false);
        
        Task AddMaterialType(List<MaterialType> materialTypes,bool unique=false);
        
        Task UpdateMaterialType(MaterialType materialType);
        
        Task UpdateMaterialType(List<MaterialType> materialTypes);

    }
}
