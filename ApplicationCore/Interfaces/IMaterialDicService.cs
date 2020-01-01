using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IMaterialDicService
    {
        Task AddMaterialDic(MaterialDic materialDic);
        Task UpdateMaterialDic(MaterialDic materialDic);
        Task DelMaterialDic(int id);
    }
}
