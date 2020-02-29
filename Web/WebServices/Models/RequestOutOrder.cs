using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Web.WebServices.Models
{
    [DataContract]
    public class RequestOutOrder
    {
        [DataMember]
        public string HeaderId { get; set; }
        [DataMember]
        public string DocumentType { get; set; }
        [DataMember]
        public string AlyNumber { get; set; }
        [DataMember]
        public string CreationBy { get; set; }
        [DataMember]
        public string BusinessEntity { get; set; }
        [DataMember]
        public string InventoryOrg { get; set; }
        [DataMember]
        public string AlyDepCode { get; set; }
        [DataMember]
        public string TransDepCode { get; set; }
        [DataMember]
        public string AlyStatusCode { get; set; }
        [DataMember]
        public string AplSourceCode { get; set; }
        [DataMember]
        public string BusinessTypeCode { get; set; }
        [DataMember]
        public string TransTypeCode { get; set; }
        [DataMember]
        public string ItemId { get; set; }
        [DataMember]
        public string CreationDate { get; set; }
        [DataMember]
        public string Remark { get; set; }
        
        [DataMember]
        public string AddUpdateFlag { get; set; }

        [DataMember]
        public List<RequestOutOrderRow> RequestOutOrderRows { get; set; }

    }
}