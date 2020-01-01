using System;
using ApplicationCore.Entities.SysManager;

namespace ApplicationCore.Specifications
{
    public class SysConfigSpecification:BaseSpecification<SysConfig>
    {
        public SysConfigSpecification(int? id,string key)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (key==null&&b.KName==key))
        {
        }
    }
}
