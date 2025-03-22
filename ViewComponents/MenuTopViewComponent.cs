using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhapClinicX.Models;

public class MenuTopViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;

    public MenuTopViewComponent(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var menus = await _menuService.GetActiveMenusAsync();

        // Tạo danh sách menu cha - con
        var menuHierarchy = menus
            .Where(m => m.ParentId == null)
            .Select(m => new Menu
            {
                MenuId = m.MenuId,
                MenuName = m.MenuName,
                Url = m.Url,
                Position = m.Position,
                IsActive = m.IsActive,
                InverseParent = menus.Where(sub => sub.ParentId == m.MenuId).ToList()
            })
            .ToList();

        return View(menuHierarchy);
    }
}
