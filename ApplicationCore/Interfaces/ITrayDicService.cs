using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface ITrayDicService
    {
        Task AddTrayDic(TrayDic trayDic);
        Task UpdateTrayDic(TrayDic trayDic);
        Task DelTrayDic(int id);
    }
}
