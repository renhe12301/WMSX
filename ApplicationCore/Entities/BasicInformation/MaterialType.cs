using System;
namespace ApplicationCore.Entities.BasicInformation
{
    public class MaterialType : BaseEntity
    {
        public string TypeName { get; set; }
        public int ParentId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }
    }
}
