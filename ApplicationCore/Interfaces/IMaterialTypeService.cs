using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IMaterialTypeService
    {
        Task AddMaterialType(MaterialType materialType);
        Task UpdateMaterialType(MaterialType materialType);
        Task DelMaterialType(List<int> ids);
        Task AssignMaterialDic(int typeId, List<int> materialDicIds);
    }
}
