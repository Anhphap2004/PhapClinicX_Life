using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace PhapClinicX.ViewComponents
{
    public class ServiceViewComponent : ViewComponent
    {
        private readonly ClinicManagementContext _context;
        public ServiceViewComponent(ClinicManagementContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var service = await _context.Services.ToListAsync();
            return View(service);
        }
    }
}
