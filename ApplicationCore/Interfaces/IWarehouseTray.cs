using System;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IWarehouseTray
    {
        Task Inventory(string trayCode);
    }
}
