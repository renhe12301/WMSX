using System;
using System.Collections.Generic;

namespace Web.ViewModels.BasicInformation
{
    public class TrayTypeViewModel
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public int ParentId { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
        public List<TrayTypeViewModel> TypeChilds = new List<TrayTypeViewModel>();
        public List<TrayDicViewModel> DicChilds = new List<TrayDicViewModel>();
    }
}
