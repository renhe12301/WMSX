using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IOrderRowBatchViewModelService
    {
        Task<ResponseResultViewModel> GetOrderRowBatchs(int? pageIndex, int? itemsPage, int? id,int?orderId,int? orderRowId,
            int? areaId,int? isRead,int? type,int? isSync,string status);
    }
}