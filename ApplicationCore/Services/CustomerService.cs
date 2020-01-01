using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class CustomerService : ICustomerService
    {

        private readonly IAsyncRepository<Customer> _customerRepository;

        public CustomerService(IAsyncRepository<Customer> customerRepository)
        {
            this._customerRepository = customerRepository;
        }

        public async Task AddCustomer(Customer customer)
        {
            Guard.Against.Null(customer, nameof(customer));
            await this._customerRepository.AddAsync(customer);
        }
    }
}
