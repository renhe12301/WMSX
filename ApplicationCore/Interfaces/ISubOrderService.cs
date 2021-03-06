using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Interfaces
{
    public interface ISubOrderService
    {
        Task SortingNoneOrder(string materialCode, double sortingCount, string trayCode, int areaId, int pyId);

        Task SortingOrder(int subOrderId, int subOrderRowId, double sortingCount, string trayCode,int areaId, int pyId);

        Task CreateTKOrder(SubOrder subOrder);

        Task CreateOrder(SubOrder subOrder);

        Task ScrapOrder(SubOrder subOrder);

        Task ScrapOrderRow(List<SubOrderRow> subOrderRows);
        
        Task OutConfirm(int subOrderId,int pyId);
    }
}