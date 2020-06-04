using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestCKLLRow
    {
        [JsonProperty(PropertyName ="lineId")]
        [DataMember]
        public string LineId { get; set; }
        
        [JsonProperty(PropertyName ="headerId")]
        [DataMember]
        public string HeaderId { get; set; }
        
        [JsonProperty(PropertyName ="lineNum")]
        [DataMember]
        public string LineNum { get; set; }
        
        [JsonProperty(PropertyName ="erpMaterialId")]
        [DataMember]
        public string MaterialId { get; set; }
        
        [JsonProperty(PropertyName ="useFor")]
        [DataMember]
        public string UseFor { get; set; }
        
        [JsonProperty(PropertyName ="inventoryCode")]
        [DataMember]
        public string InventoryCode { get; set; }
        
        [JsonProperty(PropertyName ="reqQty")]
        [DataMember]
        public string ReqQty { get; set; }
        
        [JsonProperty(PropertyName ="cancellQty")]
        [DataMember]
        public string CancelQty { get; set; }
        
        [JsonProperty(PropertyName ="requiredDate")]
        [DataMember]
        public string RequiredDate { get; set; }
        
        [JsonProperty(PropertyName ="erpTaskId")]
        [DataMember]
        public string TaskId { get; set; }
        
        [JsonProperty(PropertyName ="expenseType")]
        [DataMember]
        public string ExpenditrueType { get; set; }
        
        [JsonProperty(PropertyName ="remark")]
        [DataMember]
        public string Remark { get; set; }
        
        [JsonProperty(PropertyName ="alyStatusCode")]
        [DataMember]
        public string AlyStatusCode { get; set; }
        
        [JsonProperty(PropertyName ="addFlag")]
        [DataMember]
        public string AddFlag { get; set; }
        
        [JsonProperty(PropertyName ="result")]
        [DataMember]
        public string Result { get; set; }

        [JsonProperty(PropertyName ="errMsg")]
        [DataMember]
        public string ErrMsg { get; set; }
    }
}