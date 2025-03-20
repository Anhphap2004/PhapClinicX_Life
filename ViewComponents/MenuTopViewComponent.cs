using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PhapClinicX.ViewComponents
{
    public class MenuTopViewComponent : ViewComponent
    {
        private readonly ClinicManagementContext _context;
        public MenuTopViewComponent(ClinicManagementContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menu = await _context.Menus.Where(p=> p.IsActive == true).OrderBy(p => p.Position).ToListAsync();
            return View(menu);
        }

    }
}
