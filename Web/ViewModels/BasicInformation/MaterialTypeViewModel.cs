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
        public List<int> DicIds { get; set; }

        public List<int> Ids { get; set; }
    };
    
}
