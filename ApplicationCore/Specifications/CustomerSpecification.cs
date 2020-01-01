using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class CustomerSpecification:BaseSpecification<Customer>
    {
        public CustomerSpecification(int? id, string customerName)
             : base(b => (!id.HasValue || b.Id == id) &&
                    (customerName == null || b.CustomerName == customerName))
        {

        }
    }
}
