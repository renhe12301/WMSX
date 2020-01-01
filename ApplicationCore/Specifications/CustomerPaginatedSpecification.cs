using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class CustomerPaginatedSpecification : BaseSpecification<Customer>
    {
        public CustomerPaginatedSpecification(int skip,int take,int? id, string customerName)
             : base(b => (!id.HasValue || b.Id == id) &&
                    (customerName == null || b.CustomerName == customerName))
        {
            ApplyPaging(skip, take);
        }
    }
}
