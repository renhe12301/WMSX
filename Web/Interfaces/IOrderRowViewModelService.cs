using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IOrderRowViewModelService
    {
        Task<ResponseResultViewModel> GetOrderRows(int? pageIndex, int? itemsPage, int?id, int? orderId, string status);
    }
}