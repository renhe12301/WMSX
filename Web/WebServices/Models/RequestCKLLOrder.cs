using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestCKLLOrder
    {
        [JsonProperty(PropertyName ="headerId")]
        [DataMember]
        public string HeaderId { get; set; }
        
        [JsonProperty(PropertyName ="documentTypes")]
        [DataMember]
        public string DocumentType { get; set; }
        
        [JsonProperty(PropertyName ="alyNumber")]
        [DataMember]
        public string AlyNumber { get; set; }
        
        [JsonProperty(PropertyName ="erpCreatedBy")]
        [DataMember]
        public string CreationBy { get; set; }
        
        [JsonProperty(PropertyName ="businessEntity")]
        [DataMember]
        public string BusinessEntity { get; set; }
        
        [JsonProperty(PropertyName ="inventoryOrg")]
        [DataMember]
        public string InventoryOrg { get; set; }
        
        [JsonProperty(PropertyName ="alyDepCode")]
        [DataMember]
        public string AlyDepCode { get; set; }
        
        [JsonProperty(PropertyName ="transDepCode")]
        [DataMember]
        public string TransDepCode { get; set; }
        
        [JsonProperty(PropertyName ="alyStatusCode")]
        [DataMember]
        public string AlyStatusCode { get; set; }
        
        [JsonProperty(PropertyName ="aplSourceCode")]
        [DataMember]
        public string AplSourceCode { get; set; }
        
        [JsonProperty(PropertyName ="businessTypeCode")]
        [DataMember]
        public string BusinessTypeCode { get; set; }
        
        [JsonProperty(PropertyName ="transTypeCode")]
        [DataMember]
        public string TransTypeCode { get; set; }
        
        [JsonProperty(PropertyName ="erpItemId")]
        [DataMember]
        public string ItemId { get; set; }
        
        [JsonProperty(PropertyName ="creationDate")]
        [DataMember]
        public string CreationDate { get; set; }
        
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

        [JsonProperty(PropertyName ="transAlyLines")]
        [DataMember]
        public List<RequestCKLLRow> RequestCKLLRows = new List<RequestCKLLRow>();
    }
}