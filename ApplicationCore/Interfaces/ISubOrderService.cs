using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Interfaces
{
    public interface ISubOrderService
    {
        Task CreateOrder(SubOrder order);

        Task ScrapOrder(SubOrder order);

        Task ScrapOrderRow(List<SubOrderRow> subOrderRows);
    }
}