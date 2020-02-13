using System;
using System.Collections.Generic;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class MaterialDicIdSetSpecification : BaseSpecification<MaterialDic>
    {
        public MaterialDicIdSetSpecification(List<int> ids)
            : base(b =>(ids == null || ids.Contains(b.Id)))
        {

        }
    }
}
