using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestRKJSOrder
    {
        [JsonProperty(PropertyName ="headerId")]
        [DataMember]
        public string HeaderId { get; set; }
        
        [JsonProperty(PropertyName ="documentNumber")]
        [DataMember]
        public string DocumentNumber { get; set; }
        
        [JsonProperty(PropertyName ="documentType")]
        [DataMember]
        public string DocumentType { get; set; }
        
        [JsonProperty(PropertyName ="managerId")]
        [DataMember]
        public string ManagerId { get; set; }
        
        [JsonProperty(PropertyName ="ouCode")]
        [DataMember]
        public string OuCode { get; set; }
        
        [JsonProperty(PropertyName ="organizationCode")]
        [DataMember]
        public string OrganizationCode { get; set; }
        
        [JsonProperty(PropertyName ="vendorId")]
        [DataMember]
        public string VendorId { get; set; }
        
        [JsonProperty(PropertyName ="vendorSiteId")]
        [DataMember]
        public string VendorSiteId { get; set; }
        
        [JsonProperty(PropertyName ="businessType")]
        [DataMember]
        public string BusinessType { get; set; }
        
        [JsonProperty(PropertyName ="currency")]
        [DataMember]
        public string Currency { get; set; }
        
        [JsonProperty(PropertyName ="totalAmount")]
        [DataMember]
        public string TotalAmount { get; set; }
        
        [JsonProperty(PropertyName ="exitEntryDate")]
        [DataMember]
        public string ExitEntryDate { get; set; }
        
        [JsonProperty(PropertyName ="creationDate")]
        [DataMember]
        public string CreationDate { get; set; }
        
        [JsonProperty(PropertyName ="erpItemId")]
        [DataMember]
        public string ItemId { get; set; }
        
        [JsonProperty(PropertyName ="remark")]
        [DataMember]
        public string Remark { get; set; }
        
        [JsonProperty(PropertyName ="addFlag")]
        [DataMember]
        public string AddFlag { get; set; }
        
        [JsonProperty(PropertyName ="result")]
        [DataMember]
        public string Result { get; set; }

        [JsonProperty(PropertyName ="errMsg")]
        [DataMember]
        public string ErrMsg { get; set; }

        [JsonProperty(PropertyName ="receiptLines")]
        [DataMember]
        public List<RequestRKJSRow> RequestRKJSRows = new List<RequestRKJSRow>();


    }
}