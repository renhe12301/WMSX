using System;
namespace Web.ViewModels.BasicInformation
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Telephone { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
