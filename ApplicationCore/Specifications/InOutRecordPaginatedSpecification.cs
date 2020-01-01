using System;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Specifications
{
    public class InOutRecordPaginatedSpecification : BaseSpecification<InOutRecord>
    {
        public InOutRecordPaginatedSpecification(int skip,int take,int? type,
                                                int? inoutFlag,string sCreateTime,
                                                string eCreateTime)
            : base(b => (!type.HasValue || b.Type == type) &&
                        (!inoutFlag.HasValue||b.InOutFlag==inoutFlag)&&
                        (sCreateTime==null||b.CreateTime>=DateTime.Parse(sCreateTime))&&
                        (eCreateTime==null||b.CreateTime<=DateTime.Parse(eCreateTime)))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.ReservoirArea);
        }
    }
}
