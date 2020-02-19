using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrder(Order order);
        Task SortingOrder(int orderId, int orderRowId, int sortingCount,
                                string trayCode,int areaId);

    }
}
