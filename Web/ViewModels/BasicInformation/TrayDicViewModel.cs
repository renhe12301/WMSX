using System;
namespace Web.ViewModels.BasicInformation
{
    public class TrayDicViewModel
    {
        public int Id { get; set; }
        public string TrayCode { get; set; }
        public string TrayName { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public string Img { get; set; }
        public string CreateTime { get; set; }
        public int FullLoadRange { get; set; }
        public string Memo { get; set; }
    }
}
