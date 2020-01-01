using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class SupplierService:ISupplierService
    {
        private readonly IAsyncRepository<Supplier> _supplierRepository;

        public SupplierService(IAsyncRepository<Supplier> supplierRepository)
        {
            this._supplierRepository = supplierRepository;
        }

        public async Task AddSupplier(Supplier supplier)
        {
            Guard.Against.Null(supplier, nameof(supplier));
            await this._supplierRepository.AddAsync(supplier);
        }
    }
}
