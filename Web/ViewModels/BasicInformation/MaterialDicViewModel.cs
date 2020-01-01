using System;
namespace Web.ViewModels.BasicInformation
{
    public class MaterialDicViewModel
    {
        public int Id { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string Spec { get; set; }
        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public string Img { get; set; }
        public string CreateTime { get; set; }
        public int? UpLimit { get; set; }
        public int? DownLimit { get; set; }
        public string Memo { get; set; }
    }
}
