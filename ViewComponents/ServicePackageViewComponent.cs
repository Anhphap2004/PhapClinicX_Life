using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace PhapClinicX.ViewComponents
{
    public class ServicePackageViewComponent : ViewComponent
    {
        private readonly ClinicManagementContext _context;
      public ServicePackageViewComponent(ClinicManagementContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(bool isIndexPage)
        {
            var packages = isIndexPage
                ? await _context.ServicePackages.Where(p => p.IsActive==true).Take(3).ToListAsync() // Hiển thị 6 gói ở Index
                : await _context.ServicePackages.Where(p => p.IsActive ==true).ToListAsync(); // Hiển thị toàn bộ ở trang Service

            return View(packages);
        }
    }
}
