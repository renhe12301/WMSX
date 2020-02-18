using System.Runtime.Serialization;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestOutOrderRow
    {
        [DataMember]
        public string LineId { get; set; }
        [DataMember]
        public string HeaderId { get; set; }
        [DataMember]
        public string LineNum { get; set; }
        [DataMember]
        public string MaterialId { get; set; }
        [DataMember]
        public string UseFor { get; set; }
        [DataMember]
        public string InventoryCode { get; set; }
        [DataMember]
        public string ReqQty { get; set; }
        [DataMember]
        public string CancelQty { get; set; }
        [DataMember]
        public string RequiredDate { get; set; }
        [DataMember]
        public string TaskId { get; set; }
        [DataMember]
        public string ExpenditrueType { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}