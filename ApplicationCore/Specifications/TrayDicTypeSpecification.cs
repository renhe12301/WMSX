using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class TrayDicTypeSpecification: BaseSpecification<TrayDicType>
    {
        public TrayDicTypeSpecification(int? dicId,int? typeId,
                                            string trayCode, string trayName)
           :base(b =>   (!dicId.HasValue || b.TrayDic.Id == dicId) &&
                        (!typeId.HasValue || b.TrayType.Id == dicId) &&
                        (trayCode == null || b.TrayDic.TrayCode == trayCode) &&
                        (trayName == null||b.TrayDic.TrayName== trayName))
        {
            AddInclude(b => b.TrayDic);
            AddInclude(b => b.TrayType);
        }
    }
}
