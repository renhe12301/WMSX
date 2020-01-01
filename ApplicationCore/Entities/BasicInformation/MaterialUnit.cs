using System;
namespace ApplicationCore.Entities.BasicInformation
{
    public class MaterialUnit : BaseEntity
    {
        public int SourceId { get; set; }
        public string UnitName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Memo { get; set; }

    }
}
