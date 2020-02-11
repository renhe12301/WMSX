using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Specifications;
using System.Collections.Generic;

namespace Web.Services
{
    public class SupplierViewModelService:ISupplierViewModelService
    {
        private readonly ISupplierService _supplierService;
        private readonly IAsyncRepository<Supplier> _supplierRepository;

        public SupplierViewModelService(ISupplierService supplierService,
                                        IAsyncRepository<Supplier> supplierRepository)
        {
            this._supplierService = supplierService;
            this._supplierRepository = supplierRepository;
        }
        
        public async Task<ResponseResultViewModel> GetSuppliers(int ?pageIndex, int ?itemsPage,int? id, string supplierName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Supplier> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SupplierPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,supplierName);
                }
                else
                {
                    baseSpecification = new SupplierSpecification(null,supplierName);
                }
                var suppliers = await this._supplierRepository.ListAsync(baseSpecification);
                List<SupplierViewModel> supplierViewModels = new List<SupplierViewModel>();

                suppliers.ForEach(e =>
                {
                    SupplierViewModel supplierViewModel = new SupplierViewModel
                    {
                        SupplierName = e.SupplierName,
                        SupplierCode = e.SupplierCode,
                        TaxpayerCode = e.TaxpayerCode
                    };
                    supplierViewModels.Add(supplierViewModel);
                });
                response.Data = supplierViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }
    }
}
