using System;
using ApplicationCore.Entities.BasicInformation;
namespace ApplicationCore.Specifications
{
    public class TrayDicSpecification: BaseSpecification<TrayDic>
    {
        public TrayDicSpecification(int? id,string trayCode,string trayName)
            :base(b=>(!id.HasValue||b.Id==id)&&
                     (trayCode==null||b.TrayCode==trayCode)&&
                     (trayName==null||b.TrayName==trayName))
        {
        }
    }
}
