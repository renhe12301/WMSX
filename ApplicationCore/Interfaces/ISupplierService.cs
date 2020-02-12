using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Interfaces
{
    public interface ISupplierService
    {
        Task AddSupplier(Supplier supplier,bool unique=false);
        Task AddSupplier(List<Supplier> suppliers,bool unique=false);
        Task UpdateSupplier(Supplier supplier);
        Task UpdateSupplier(List<Supplier> suppliers);
    }
}
