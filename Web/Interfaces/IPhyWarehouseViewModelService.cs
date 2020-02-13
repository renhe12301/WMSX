using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IPhyWarehouseViewModelService
    {
        Task<ResponseResultViewModel> GetPhyWarehouses(int? pageIndex, int? itemsPage, int? id,string phyName);
        
        Task<ResponseResultViewModel> GetPhyWarehouseTrees();
    }
}