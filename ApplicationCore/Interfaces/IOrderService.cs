using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderManager;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task CreateOutOrder(Order order);
        Task OrderOut(int orderId, int orderRowId,int areaId , int sortingCount,int type, string tag);

        Task CloseOrder(int orderId, string tag);
        
        Task CloseOrderRow(int orderRowId, string tag);

    }
}
