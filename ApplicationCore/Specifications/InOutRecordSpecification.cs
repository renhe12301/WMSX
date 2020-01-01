﻿using System;
using ApplicationCore.Entities.FlowRecord;

namespace ApplicationCore.Specifications
{
    public class InOutRecordSpecification : BaseSpecification<InOutRecord>
    {
        public InOutRecordSpecification(int? type,
                                                int? inoutFlag,int? isRead, string sCreateTime,
                                                string eCreateTime)
            : base(b => (!type.HasValue || b.Type == type) &&
                        (!inoutFlag.HasValue || b.InOutFlag == inoutFlag) &&
                        (!isRead.HasValue||b.IsRead==isRead)&&
                        (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                        (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)))
        {
            AddInclude(b => b.Warehouse);
            AddInclude(b => b.ReservoirArea);
        }
    }
}
