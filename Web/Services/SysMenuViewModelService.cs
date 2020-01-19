using System;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using Web.ViewModels.OrganizationManager;

namespace Web.Services
{
    public class SysMenuViewModelService:ISysMenuViewModelService
    {
        private readonly ISysMenuService _sysMenuService;
        private readonly IAsyncRepository<RoleMenu> _roleMenuRepository;
        private readonly IAsyncRepository<SysMenu> _menuRepository;
        public SysMenuViewModelService(IAsyncRepository<RoleMenu> roleMenuRepository,
            IAsyncRepository<SysMenu> menuRepository,
            ISysMenuService sysMenuService)
        {
            this._roleMenuRepository = roleMenuRepository;
            this._menuRepository = menuRepository;
            this._sysMenuService = sysMenuService;
        }

        public async Task<ResponseResultViewModel> GetMenus(int roleId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                BaseSpecification<RoleMenu> baseSpecification = new RoleMenuSpecification(roleId);
                var result = await this._roleMenuRepository.ListAsync(baseSpecification);
                List<RoleMenuViewModel> roleMenuViewModels = new List<RoleMenuViewModel>();
                result.ForEach(r =>
                {
                    RoleMenuViewModel roleMenuViewModel = new RoleMenuViewModel
                    {
                        RoleId=r.SysRoleId,
                        MenuId=r.SysMenuId,
                        RoleName=r.SysRole.RoleName,
                        MenuName=r.SysMenu.MenuName
                    };
                    roleMenuViewModels.Add(roleMenuViewModel);
                });
                response.Data = roleMenuViewModels;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<ResponseResultViewModel> GetMenuTrees(int rootId)
        {
            ResponseResultViewModel response = new ResponseResultViewModel { Code = 200 };
            try
            {
                var menuSpec = new SysMenuSpecification(null, null, null,null);
                var allMenus = await this._menuRepository.ListAsync(menuSpec);
                if (allMenus.Count == 0) throw new Exception(string.Format("菜单编号{0}不存在", rootId));
                var rootMenu = allMenus.Find(m=>m.Id==rootId);
                TreeViewModel current = new TreeViewModel
                {
                    Id = rootMenu.Id,
                    ParentId = rootMenu.ParentId,
                    Name = rootMenu.MenuName,
                    Type = rootMenu.IsLeaf==0?"dir":"leaf"
                };

                MenuTree(current, current.Children,allMenus);
                response.Data = current;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Data = ex.Message;
            }
            return response;
        }

        private void MenuTree(TreeViewModel current, List<TreeViewModel> childs, List<SysMenu> data)
        {
            var menus = data.FindAll(m => m.ParentId == current.Id);

            menus.ForEach((menu) =>
            {
                TreeViewModel child = new TreeViewModel
                {
                    Id = menu.Id,
                    ParentId = current.Id,
                    Name = menu.MenuName,
                    Type = menu.IsLeaf==0?"dir":"leaf"
                };
                childs.Add(child);
                MenuTree(child, child.Children, data);
            });
        }

    }
}
