using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
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

        public async Task AddSupplier(Supplier supplier,bool unique=false)
        {
            Guard.Against.Null(supplier, nameof(supplier));
            Guard.Against.Zero(supplier.Id,nameof(supplier.Id));
            Guard.Against.NullOrEmpty(supplier.SupplierCode,nameof(supplier.SupplierCode));
            Guard.Against.NullOrEmpty(supplier.SupplierName,nameof(supplier.SupplierName));
            if (unique)
            {
                SupplierSpecification supplierSpec = new SupplierSpecification(supplier.Id, null);
                List<Supplier> suppliers = await this._supplierRepository.ListAsync(supplierSpec);
                if (suppliers.Count == 0)
                   await this._supplierRepository.AddAsync(supplier);
            }
            else
            {
                await this._supplierRepository.AddAsync(supplier);
            }

        }

        public async Task AddSupplier(List<Supplier> suppliers, bool unique = false)
        {
            Guard.Against.Null(suppliers, nameof(suppliers));
            if (unique)
            {
                List<Supplier> adds=new List<Supplier>();
                suppliers.ForEach(async (s) =>
                {
                    SupplierSpecification supplierSpec = new SupplierSpecification(s.Id, null);
                    List<Supplier> suppliers = await this._supplierRepository.ListAsync(supplierSpec);
                    if (suppliers.Count == 0)
                        adds.Add(s);
                });
                if (adds.Count > 0)
                    await this._supplierRepository.AddAsync(adds);
            }
            else
            {
                await this._supplierRepository.AddAsync(suppliers);
            }
        }

        public async Task UpdateSupplier(Supplier supplier)
        {
            Guard.Against.Null(supplier,nameof(supplier));
            await this._supplierRepository.UpdateAsync(supplier);

        }

        public async Task UpdateSupplier(List<Supplier> suppliers)
        {
            Guard.Against.Null(suppliers,nameof(suppliers));
            await this._supplierRepository.UpdateAsync(suppliers);
        }
    }
}
