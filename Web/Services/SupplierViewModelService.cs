using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Dynamic;

namespace Web.Services
{
    public class SupplierViewModelService:ISupplierViewModelService
    {
        private readonly ISupplierService _supplierService;
        private readonly IAsyncRepository<Supplier> _supplierRepository;
        private readonly IAsyncRepository<SupplierSite> _supplierSiteRepository;

        public SupplierViewModelService(ISupplierService supplierService,
                                        IAsyncRepository<Supplier> supplierRepository,
                                        IAsyncRepository<SupplierSite> supplierSiteRepository)
        {
            this._supplierService = supplierService;
            this._supplierRepository = supplierRepository;
            this._supplierSiteRepository = supplierSiteRepository;
        }
        
        public async Task<ResponseResultViewModel> GetSuppliers(int ?pageIndex, int ?itemsPage,int? id, string supplierName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Supplier> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
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
                        Id = e.Id,
                        SupplierName = e.SupplierName,
                        SupplierCode = e.SupplierCode,
                        TaxpayerCode = e.TaxpayerCode
                    };
                    supplierViewModels.Add(supplierViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._supplierRepository.CountAsync(new SupplierPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,supplierName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = supplierViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = supplierViewModels;
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetSupplierSites(int? pageIndex, int? itemsPage, int? id, string supplierName,
                                                                    int? supplierId,int? ouId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<SupplierSite> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > -1 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new SupplierSitePaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,supplierName,supplierId,ouId);
                }
                else
                {
                    baseSpecification = new SupplierSiteSpecification(id,supplierName,supplierId,ouId);
                }
                var suppliers = await this._supplierSiteRepository.ListAsync(baseSpecification);
                List<SupplierSiteViewModel> supplierSiteViewModels = new List<SupplierSiteViewModel>();

                suppliers.ForEach(e =>
                {
                    SupplierSiteViewModel supplierSiteViewModel = new SupplierSiteViewModel
                    {
                        Id = e.Id,
                        SiteName = e.SiteName,
                        SupplierId = e.Supplier?.Id,
                        SupplierName = e.Supplier?.SupplierName,
                        Address = e.Address,
                        Contact = e.Contact,
                        TelPhone = e.TelPhone,
                        OUId = e.OUId,
                        OUName = e.OU?.OUName
                    };
                    supplierSiteViewModels.Add(supplierSiteViewModel);
                });
                if (pageIndex > -1 && itemsPage > 0)
                {
                    var count = await this._supplierRepository.CountAsync(new SupplierPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id,supplierName));
                    dynamic dyn = new ExpandoObject();
                    dyn.rows = supplierSiteViewModels;
                    dyn.total = count;
                    response.Data = dyn;
                }
                else
                {
                    response.Data = supplierSiteViewModels;
                }
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
