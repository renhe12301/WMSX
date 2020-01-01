using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface ITrayTypeService
    {
        Task AddTrayType(TrayType trayType);
        Task UpdateTrayType(TrayType trayType);
        Task DelTrayType(int id);
        Task AssignTrayDic(int typeId, List<int> trayDicIds);
    }
}
