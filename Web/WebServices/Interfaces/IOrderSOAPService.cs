

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
        /// <param name="requestEnterOrder"></param>
        /// <returns></returns>
        [OperationContract]
        Task<ResponseResult> CreateEnterOrder(RequestEnterOrder requestEnterOrder);
        
        /// <summary>
        /// 创建出库订单[出库领料、出库退料]
        /// </summary>
        /// <param name="requestOutOrder"></param>
        /// <returns></returns>
        [OperationContract]
        Task<ResponseResult> CreateOutOrder(RequestOutOrder requestOutOrder);
    }
}