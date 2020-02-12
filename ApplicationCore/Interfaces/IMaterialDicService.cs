using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IMaterialDicService
    {
        Task AddMaterialDic(MaterialDic materialDic,bool unique=false);
        Task UpdateMaterialDic(MaterialDic materialDic);
        Task AddMaterialDic(List<MaterialDic> materialDics,bool unique=false);
        Task UpdateMaterialDic(List<MaterialDic> materialDics);
    }
}
