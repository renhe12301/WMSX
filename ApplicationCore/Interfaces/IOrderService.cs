using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrder(Order order);
        Task SortingOrder2Area(int orderId, int orderRowId, int sortingCount,
                                                   string trayCode);

    }
}
