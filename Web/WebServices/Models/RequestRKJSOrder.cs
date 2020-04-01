using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestRKJSOrder
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
        public string AddFlag { get; set; }
        
        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public string ErrMsg { get; set; }

        [DataMember]
        public RequestRKJSRow[] RequestRKJSRows { get; set; }
    }
}