using System;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public class TreeViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public List<TreeViewModel> Children = new List<TreeViewModel>();
    }

}
