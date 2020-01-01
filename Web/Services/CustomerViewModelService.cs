using System;
using Web.Interfaces;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using System.Threading.Tasks;
using Web.ViewModels;
using Web.ViewModels.BasicInformation;
using ApplicationCore.Specifications;
using System.Collections.Generic;

namespace Web.Services
{
    public class CustomerViewModelService:ICustomerViewModelService
    {
        private readonly IAsyncRepository<Customer> _customerRepository;
        private readonly ICustomerService _customerService;

        public CustomerViewModelService(IAsyncRepository<Customer> customerRepository,
                                         ICustomerService customerService)
        {
            this._customerRepository = customerRepository;
            this._customerService = customerService;
        }

        public async Task<ResponseResultViewModel> AddCustomer(CustomerViewModel customerViewModel)
        {
            ResponseResultViewModel responseResultViewModel = new ResponseResultViewModel { Code = 200 };
            try
            {
                Customer customer = new Customer
                {
                    Address = customerViewModel.Address,
                    Contact = customerViewModel.Contact,
                    CreateTime = DateTime.Now,
                    CustomerName = customerViewModel.CustomerName,
                    Memo = customerViewModel.Memo,
                    Telephone = customerViewModel.Telephone
                };
                await this._customerService.AddCustomer(customer);
            }
            catch (Exception ex)
            {
                responseResultViewModel.Code = 500;
                responseResultViewModel.Data = ex.Message;
            }
            return responseResultViewModel;
        }

        public async Task<ResponseResultViewModel> GetCustomers(int? pageIndex, int? itemsPage,int? id, string customerName)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<Customer> baseSpecification = null;

                if (pageIndex.HasValue && pageIndex > 0 && itemsPage.HasValue && itemsPage > 0)
                {
                    baseSpecification = new CustomerPaginatedSpecification(pageIndex.Value, itemsPage.Value,
                        id, customerName);
                }
                else
                {
                    baseSpecification = new CustomerSpecification(id, customerName);
                }
                var customers = await this._customerRepository.ListAsync(baseSpecification);
                List<CustomerViewModel> customerViewModels = new List<CustomerViewModel>();

                customers.ForEach(e =>
                {
                    CustomerViewModel cusmtomerViewModel = new CustomerViewModel
                    {
                        Address = e.Address,
                        Contact = e.Contact,
                        CustomerName = e.CustomerName,
                        Telephone = e.Telephone
                    };
                    customerViewModels.Add(cusmtomerViewModel);
                });
                response.Data = customerViewModels;
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
