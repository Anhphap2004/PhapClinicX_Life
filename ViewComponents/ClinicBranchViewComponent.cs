using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace PhapClinicX.ViewComponents
{
    public class ClinicBranchViewComponent : ViewComponent
    {
        private readonly ClinicManagementContext _context;

        public ClinicBranchViewComponent(ClinicManagementContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default")
        {
            var branches = await _context.PhongKhams
                .Where(p => p.Isactive == true)
                .ToListAsync();

            // Render View theo tham số truyền vào
            return View(viewName, branches);
        }
    }
}
