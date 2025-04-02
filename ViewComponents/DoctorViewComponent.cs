using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace PhapClinicX.ViewComponents
{
    public class DoctorViewComponent : ViewComponent
    {
        private readonly ClinicManagementContext _context;
        public DoctorViewComponent(ClinicManagementContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
           var doctors = await _context.DoctorProfiles.Where(p => p.Isactive == true).OrderBy(p=>p.Experience).Take(6).ToListAsync();
            return View(doctors);
        }
    }
}
