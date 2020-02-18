using System.Runtime.Serialization;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestEnterOrderRow
    {
        [DataMember]
        public string LineId { get; set; }
        [DataMember]
        public string HeaderId { get; set; }
        [DataMember]
        public string LineNumber { get; set; }
        [DataMember]
        public string MaterialId { get; set; }
        [DataMember]
        public string ProcessingQuantity { get; set; }
        [DataMember]
        public string Price { get; set; }
        [DataMember]
        public string Amount { get; set; }
        [DataMember]
        public string TaskId { get; set; }
        [DataMember]
        public string ExpenditrueType { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}