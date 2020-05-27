using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Interfaces
{
    public interface ISubOrderService
    {
        
        Task SortingOrder(int subOrderId, int subOrderRowId, int sortingCount, string trayCode,int areaId, string tag);
        
        Task CreateOrder(SubOrder subOrder);

        Task ScrapOrder(SubOrder subOrder);

        Task ScrapOrderRow(List<SubOrderRow> subOrderRows);
        
        Task OutConfirm(int subOrderId);
    }
}