using System;
using ApplicationCore.Entities.AuthorityManager;
namespace ApplicationCore.Specifications
{
    public class SysMenuSpecification:BaseSpecification<SysMenu>
    {
        public SysMenuSpecification(int? id,string menuName,int? parentId,int? isLeaf)
            :base(b=>(!id.HasValue||b.Id==id)&&
                  (menuName==null||b.MenuName==menuName)&&
                  (!parentId.HasValue||b.ParentId==parentId)&&
                  (!isLeaf.HasValue||b.IsLeaf==isLeaf))
        {

        }
    }
}
