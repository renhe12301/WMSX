

using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Web.WebServices.Models;

namespace Web.WebServices.Interfaces
{
    [ServiceContract]
    public interface IOrderSOAPService
    {
        /// <summary>
        /// 创建入库订单[入库接收]
        /// </summary>
        /// <param name="RequestRKJSOrders"></param>
        /// <returns></returns>
        [OperationContract]
        Task<ResponseResult> CreateRKJSOrder(RequestRKJSOrder[] RequestRKJSOrders);
        //
        // /// <summary>
        // /// 创建出库订单[出库领料、出库退料]
        // /// </summary>
        // /// <param name="RequestCKLLOrders"></param>
        // /// <returns></returns>
        [OperationContract]
        Task<ResponseResult> CreateCKLLOrder(RequestCKLLOrder[] RequestCKLLOrders);

        /// <summary>
        /// 测试使用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [OperationContract]
        string Hello(string name);
    }
}