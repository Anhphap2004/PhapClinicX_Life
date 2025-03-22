using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace PhapClinicX.ViewComponents
{
    public class ClinicBranchViewComponent : ViewComponent
    {
        private readonly ClinicManagementContext _context;
        public ClinicBranchViewComponent(ClinicManagementContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var branches = await _context.PhongKhams.Where(p => p.Isactive == true).ToListAsync();
            return View(branches);
        }

    }
}
