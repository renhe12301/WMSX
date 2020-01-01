using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class TrayDicTypePaginatedSpecification: BaseSpecification<TrayDicType>
    {
        public TrayDicTypePaginatedSpecification(int skip,int take,int? dicId,int? typeId,
                                            string trayCode, string trayName)
           :base(b =>   (!dicId.HasValue || b.TrayDic.Id == dicId) &&
                        (!typeId.HasValue || b.TrayType.Id == dicId) &&
                        (trayCode == null || b.TrayDic.TrayCode == trayCode) &&
                        (trayName == null||b.TrayDic.TrayName== trayName))
        {
            ApplyPaging(skip, take);
            AddInclude(b => b.TrayDic);
            AddInclude(b => b.TrayType);
        }
    }
}
