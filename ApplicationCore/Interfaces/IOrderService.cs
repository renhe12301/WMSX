using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOutOrder(Order order);
        Task SortingOrder(int orderId, int orderRowId, int sortingCount,int badCount,
                                string trayCode,int areaId);

        Task OrderOut(int orderId, int orderRowId,int areaId , int sortingCount,int type);

    }
}
