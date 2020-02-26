using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestEnterOrder
    {
        [DataMember]
        public string HeaderId { get; set; }
        [DataMember]
        public string DocumentNumber { get; set; }
        [DataMember]
        public string DocumentType { get; set; }
        [DataMember]
        public string ManagerId { get; set; }
        [DataMember]
        public string OuCode { get; set; }
        [DataMember]
        public string OrganizationCode { get; set; }
        [DataMember]
        public string VendorId { get; set; }
        [DataMember]
        public string VendorSiteId { get; set; }
        [DataMember]
        public string BusinessType { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string TotalAmount { get; set; }
        [DataMember]
        public string ExitEntryDate { get; set; }
        [DataMember]
        public string CreationDate { get; set; }
        [DataMember]
        public string ItemId { get; set; }
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public string AddUpdateFlag { get; set; }
        [DataMember]
        public List<RequestEnterOrderRow> RequestEnterOrderRows { get; set; }
    }
}