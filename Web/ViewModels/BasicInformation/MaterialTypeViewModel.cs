using System;
using System.Collections.Generic;

namespace Web.ViewModels.BasicInformation
{
    public class MaterialTypeViewModel
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public int ParentId { get; set; }
        public string CreateTime { get; set; }
        public string Memo { get; set; }
        public List<MaterialTypeViewModel> TypeChilds = new List<MaterialTypeViewModel>();
        public List<MaterialDicViewModel> DicChilds = new List<MaterialDicViewModel>();
    }
}
