using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface ISupplierService
    {
        Task AddSupplier(Supplier supplier);
    }
}
