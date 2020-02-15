using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EBSProjectSpecification:BaseSpecification<EBSProject>
    {
        public EBSProjectSpecification(int? id,string projectName,int? ouId, 
                string sCreateTime, string eCreateTime,
                string sEndTime, string eEndTime)
            : base(b =>(!id.HasValue || b.Id == id)&&
                       (projectName == null || b.ProjectName.Contains(projectName)) &&
                       (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                       (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)) &&
                       (sEndTime == null || b.EndTime >= DateTime.Parse(sEndTime)) &&
                       (eEndTime == null || b.EndTime <= DateTime.Parse(eEndTime)))
        {
           
        }
    }
}
