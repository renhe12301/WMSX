using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ISysMenuService
    {
        Task AssignMenu(int roleId, List<int> menuIds);
    }
}
