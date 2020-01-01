using System;
using System.Collections.Generic;

namespace Web.ViewModels.BasicInformation
{
    public class MaterialDicTypeAreaViewModel
    {
        public int Id { get; set; }
        public int MaterialTypeId { get; set; }
        public int WarehouseId { get; set; }
        public int ReservoirAreaId { get; set; }
        public List<int> MaterialTypeIds { get; set; }
    }
}
