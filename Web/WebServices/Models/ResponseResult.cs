using System.Runtime.Serialization;

namespace Web.WebServices.Models
{
    [DataContract]
    public class ResponseResult
    {
        [DataMember]
        public int Code { get; set; }
        [DataMember]
        public object Data { get; set; }
    }
}