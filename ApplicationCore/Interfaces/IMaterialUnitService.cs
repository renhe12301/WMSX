using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface IMaterialUnitService
    {
        Task AddMaterialUnit(MaterialUnit materialUnit);
        Task UpdateMaterialUnit(MaterialUnit materialUnit);
        Task DelMaterialUnit(int id);
    }
}
