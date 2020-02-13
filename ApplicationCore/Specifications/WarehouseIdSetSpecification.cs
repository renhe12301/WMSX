﻿using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class WarehouseIdSpecification:BaseSpecification<Warehouse>
    {
        public WarehouseIdSpecification(List<int> ids)
            : base(b =>(ids == null || ids.Contains(b.Id)))
        {
            
        }
    }
}
