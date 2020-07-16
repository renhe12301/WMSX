using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestRKJSRow
    {
        
        [JsonProperty(PropertyName ="lineId")]
        [DataMember]
        public string LineId { get; set; }
        
        [JsonProperty(PropertyName ="headerId")]
        [DataMember]
        public string HeaderId { get; set; }
        
        [JsonProperty(PropertyName ="lineNumber")]
        [DataMember]
        public string LineNumber { get; set; }
        
        [JsonProperty(PropertyName ="materialId")]
        [DataMember]
        public string MaterialId { get; set; }
        
        [JsonProperty(PropertyName ="processingQuantity")]
        [DataMember]
        public string ProcessingQuantity { get; set; }
        
        [JsonProperty(PropertyName ="price")]
        [DataMember]
        public string Price { get; set; }
        
        [JsonProperty(PropertyName ="amount")]
        [DataMember]
        public string Amount { get; set; }
        
        [JsonProperty(PropertyName ="taskId")]
        [DataMember]
        public string TaskId { get; set; }

        [JsonProperty(PropertyName = "itemId")]
        [DataMember]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName ="expenditureType")]
        [DataMember]
        public string ExpenditrueType { get; set; }
        
        [JsonProperty(PropertyName ="remark")]
        [DataMember]
        public string Remark { get; set; }
        
        [JsonProperty(PropertyName ="addFlag")]
        [DataMember]
        public string AddFlag { get; set; }
        
        [JsonProperty(PropertyName = "relatedId")]
        [DataMember]
        public string RelatedId { get; set; }
        
        [JsonProperty(PropertyName ="result")]
        [DataMember]
        public string Result { get; set; }

        [JsonProperty(PropertyName ="errMsg")]
        [DataMember]
        public string ErrMsg { get; set; }
        
    }
}