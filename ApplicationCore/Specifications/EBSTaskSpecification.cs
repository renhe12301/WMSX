using System;
using ApplicationCore.Entities.BasicInformation;

namespace ApplicationCore.Specifications
{
    public class EBSTaskSpecification:BaseSpecification<EBSTask>
    {
        public EBSTaskSpecification(int? id,string taskName,int? projectId, 
                string sCreateTime, string eCreateTime,
                string sEndTime, string eEndTime)
            : base(b =>(!id.HasValue || b.Id == id)&&
                       (taskName == null || b.TaskName.Contains(taskName)) &&
                       (projectId == null || b.ProjectId==projectId) &&
                       (sCreateTime == null || b.CreateTime >= DateTime.Parse(sCreateTime)) &&
                       (eCreateTime == null || b.CreateTime <= DateTime.Parse(eCreateTime)) &&
                       (sEndTime == null || b.EndTime >= DateTime.Parse(sEndTime)) &&
                       (eEndTime == null || b.EndTime <= DateTime.Parse(eEndTime)))
        {
           
        }
    }
}
